using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.BreakoutRoom.Dto;
using PaderConference.Core.Services.BreakoutRoom.Naming;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Messages;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.BreakoutRoom
{
    public class BreakoutRoomService : ConferenceService
    {
        private readonly IPermissionsService _permissionsService;
        private readonly IRoomManagement _roomManagement;
        private readonly ILogger<BreakoutRoomService> _logger;
        private readonly ISynchronizedObject<BreakoutRoomSyncObject> _synchronizedObject;
        private readonly AsyncLock _breakoutLock = new AsyncLock();
        private CancellationTokenSource? _currentAutoCloseTokenSource;

        private readonly ConcurrentDictionary<string, Room> _currentBreakoutRooms = new();

        private readonly IRoomNamingStrategy _namingStrategy = new NatoRoomNamingStrategy();

        public BreakoutRoomService(ISynchronizationManager synchronizationManager,
            IPermissionsService permissionsService, IRoomManagement roomManagement, ILogger<BreakoutRoomService> logger)
        {
            _permissionsService = permissionsService;
            _roomManagement = roomManagement;
            _logger = logger;
            _synchronizedObject = synchronizationManager.Register("breakoutRooms", new BreakoutRoomSyncObject());
        }

        public override ValueTask InitializeAsync()
        {
            _roomManagement.RoomsRemoved += RoomManagementOnRoomsRemoved;
            return new ValueTask();
        }

        public override ValueTask DisposeAsync()
        {
            _roomManagement.RoomsRemoved -= RoomManagementOnRoomsRemoved;
            return new ValueTask();
        }

        private void RoomManagementOnRoomsRemoved(object? sender, IReadOnlyList<string> e)
        {
            foreach (var roomId in e) _currentBreakoutRooms.TryRemove(roomId, out _);
        }

        public async ValueTask OpenBreakoutRooms(IServiceMessage<OpenBreakoutRoomsDto> message)
        {
            using var _ = _logger.BeginMethodScope();
            using (await _breakoutLock.LockAsync())
            {
                if (_synchronizedObject.Current.Active != null)
                {
                    _logger.LogDebug("Breakout rooms are already open");
                    await message.ResponseError(BreakoutRoomError.AlreadyOpen);
                    return;
                }

                if (message.Payload.AssignedRooms?.Length > message.Payload.Amount)
                {
                    _logger.LogDebug("Cannot assign participants to {count} rooms if only {amount} groups are rooms.",
                        message.Payload.AssignedRooms.Length, message.Payload.Amount);
                    await message.ResponseError(BreakoutRoomError.CannotAssignParticipants);
                    return;
                }

                if (message.Payload.Amount <= 0)
                {
                    _logger.LogDebug("Cannot create zero or less breakout rooms. Amount given: {amount}",
                        message.Payload.Amount);
                    await message.ResponseError(BreakoutRoomError.AmountMustBePositiveNumber);
                    return;
                }

                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                {
                    _logger.LogDebug("Permissions denied, cannot create or remove rooms.");
                    await message.ResponseError(RoomsError.PermissionToCreateRoomDenied);
                    return;
                }

                var deadline = message.Payload.Duration == null
                    ? (DateTimeOffset?) null
                    : DateTimeOffset.UtcNow.Add(message.Payload.Duration.Value);

                var state = new ActiveBreakoutRoomState
                {
                    Amount = message.Payload.Amount, Deadline = deadline, Description = message.Payload.Description,
                };
                await ApplyState(state);

                if (message.Payload.AssignedRooms?.Length > 0)
                {
                    var sortedRooms = _currentBreakoutRooms.Values
                        .OrderBy(x => _namingStrategy.ParseIndex(x.DisplayName)).ToList();

                    if (message.Payload.AssignedRooms.Length > sortedRooms.Count)
                    {
                        await message.ResponseError(BreakoutRoomError.CannotAssignParticipants);
                        return;
                    }

                    try
                    {
                        for (var i = 0; i < message.Payload.AssignedRooms.Length; i++)
                        {
                            var room = sortedRooms[i];

                            foreach (var participantId in message.Payload.AssignedRooms[i])
                                await _roomManagement.SetRoom(participantId, room.RoomId);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "An error occurred assigning participants to rooms.");
                        await message.ResponseError(BreakoutRoomError.AssigningParticipantsFailed);
                    }
                }
            }
        }

        public async ValueTask CloseBreakoutRooms(IServiceMessage message)
        {
            using var _ = _logger.BeginMethodScope();
            using (await _breakoutLock.LockAsync())
            {
                if (_synchronizedObject.Current.Active == null)
                {
                    _logger.LogDebug("Breakout rooms are not open");
                    await message.ResponseError(BreakoutRoomError.NotOpen);
                    return;
                }

                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                {
                    _logger.LogDebug("Permissions denied, cannot create or remove rooms.");
                    await message.ResponseError(RoomsError.PermissionToCreateRoomDenied);
                    return;
                }

                await ApplyState(null);
            }
        }

        public async ValueTask ChangeBreakoutRooms(IServiceMessage<JsonPatchDocument<BreakoutRoomsOptions>> message)
        {
            using var _ = _logger.BeginMethodScope();
            using (await _breakoutLock.LockAsync())
            {
                var current = _synchronizedObject.Current;
                if (current.Active == null)
                {
                    _logger.LogDebug("Breakout rooms must be opened first");
                    await message.ResponseError(BreakoutRoomError.NotOpen);
                    return;
                }

                // a placeholder to detect if the duration has changed
                // as the duration cannot be negative values, this is safe to use
                var unchangedPlaceholder = TimeSpan.FromDays(-3.14);

                var newOptions = new BreakoutRoomsOptions
                {
                    Amount = current.Active.Amount,
                    Description = current.Active.Description,
                    Duration = unchangedPlaceholder,
                };

                message.Payload.ApplyTo(newOptions);

                if (current.Active.Amount <= 0)
                {
                    _logger.LogDebug("Cannot create zero or less breakout rooms. Amount given: {amount}",
                        current.Active);
                    await message.ResponseError(BreakoutRoomError.AmountMustBePositiveNumber);
                }

                DateTimeOffset? deadline = null;

                // the duration has not changed
                if (newOptions.Duration == unchangedPlaceholder)
                {
                    deadline = current.Active.Deadline;
                }
                else
                {
                    if (newOptions.Duration != null)
                        deadline = DateTimeOffset.UtcNow.Add(newOptions.Duration.Value);
                }

                var state = new ActiveBreakoutRoomState
                {
                    Amount = newOptions.Amount, Deadline = deadline, Description = newOptions.Description,
                };
                await ApplyState(state);
            }
        }

        private async Task ApplyState(ActiveBreakoutRoomState? state)
        {
            // IMPORTANT: Lock must be acquired!

            // cancel
            _currentAutoCloseTokenSource?.Cancel();
            _currentAutoCloseTokenSource?.Dispose();
            _currentAutoCloseTokenSource = null;

            if (state == null)
            {
                await _roomManagement.RemoveRooms(_currentBreakoutRooms.Keys.ToList());
            }
            else
            {
                // adjust rooms
                if (state.Amount > _currentBreakoutRooms.Count)
                {
                    var createdRooms = await CreateMissingRooms(state.Amount - _currentBreakoutRooms.Count,
                        _namingStrategy);

                    foreach (var createdRoom in createdRooms)
                        _currentBreakoutRooms.TryAdd(createdRoom.RoomId, createdRoom);
                }
                else if (state.Amount < _currentBreakoutRooms.Count)
                {
                    await RemoveRooms(_currentBreakoutRooms.Count - state.Amount, _namingStrategy);
                }

                // adjust timer
                if (state.Deadline != null)
                {
                    var wait = state.Deadline.Value - DateTimeOffset.UtcNow;
                    if (wait < TimeSpan.Zero)
                    {
                        await ApplyState(null);
                        return;
                    }

                    _currentAutoCloseTokenSource = new CancellationTokenSource();
                    Task.Delay(wait, _currentAutoCloseTokenSource.Token).ContinueWith(async task =>
                    {
                        if (task.IsCanceled) return;

                        _logger.LogInformation("Breakout room timer ran out, close all breakout rooms");
                        try
                        {
                            await ApplyState(null);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Error closing all breakout rooms after timer ran out.");
                        }
                    }).Forget();
                }
            }

            await _synchronizedObject.Update(new BreakoutRoomSyncObject {Active = state});
        }

        private async Task<IReadOnlyList<Room>> CreateMissingRooms(int amount, IRoomNamingStrategy namingStrategy)
        {
            var createRooms = new List<CreateRoomMessage>();

            var currentPos = 0;
            for (var i = 0; i < amount; i++)
            {
                // find first unused name
                string name;
                do
                {
                    name = namingStrategy.GetName(currentPos++);
                } while (_currentBreakoutRooms.Any(x => x.Value.DisplayName == name));

                createRooms.Add(new CreateRoomMessage(name));
            }

            return await _roomManagement.CreateRooms(createRooms);
        }

        private async Task<IReadOnlyList<string>> RemoveRooms(int amount, IRoomNamingStrategy namingStrategy)
        {
            var state = _roomManagement.State;

            // order rooms by least participants, then by their naming index (desc)
            var removeRooms = _currentBreakoutRooms.OrderBy(x => state.Participants.Count(p => p.Value == x.Key))
                .ThenByDescending(x => namingStrategy.ParseIndex(x.Value.DisplayName)).Take(amount).Select(x => x.Key)
                .ToList();

            await _roomManagement.RemoveRooms(removeRooms);
            return removeRooms;
        }
    }
}
