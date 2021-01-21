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
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.BreakoutRoom.Dto;
using PaderConference.Core.Services.BreakoutRoom.Naming;
using PaderConference.Core.Services.BreakoutRoom.Requests;
using PaderConference.Core.Services.BreakoutRoom.Validation;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Requests;
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

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            _roomManagement.RoomsRemoved -= RoomManagementOnRoomsRemoved;
        }

        private void RoomManagementOnRoomsRemoved(object? sender, IReadOnlyList<string> e)
        {
            foreach (var roomId in e) _currentBreakoutRooms.TryRemove(roomId, out _);
        }

        public async ValueTask<SuccessOrError> OpenBreakoutRooms(IServiceMessage<OpenBreakoutRoomsRequest> message)
        {
            using var _ = _logger.BeginMethodScope();
            using (await _breakoutLock.LockAsync())
            {
                if (_synchronizedObject.Current.Active != null) return BreakoutRoomError.AlreadyOpen;

                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                {
                    _logger.LogDebug("Permissions denied, cannot create or remove rooms.");
                    return CommonError.PermissionDenied(PermissionsList.Rooms.CanCreateAndRemove);
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
                        return BreakoutRoomError.AssigningParticipantsFailed;

                    for (var i = 0; i < message.Payload.AssignedRooms.Length; i++)
                    {
                        var room = sortedRooms[i];

                        foreach (var participantId in message.Payload.AssignedRooms[i])
                        {
                            var result = await _roomManagement.SetRoom(participantId, room.RoomId);
                            if (!result.Success)
                                _logger.LogWarning(
                                    "Failed to assign participant {participantId} to room {roomId}: {error}",
                                    participantId, room.RoomId, result.Error);
                        }
                    }

                    return SuccessOrError.Succeeded;
                }

                return SuccessOrError.Succeeded;
            }
        }

        public async ValueTask<SuccessOrError> CloseBreakoutRooms(IServiceMessage message)
        {
            using var _ = _logger.BeginMethodScope();
            using (await _breakoutLock.LockAsync())
            {
                if (_synchronizedObject.Current.Active == null) return BreakoutRoomError.NotOpen;

                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                    return CommonError.PermissionDenied(PermissionsList.Rooms.CanCreateAndRemove);

                await ApplyState(null);
                return SuccessOrError.Succeeded;
            }
        }

        public async ValueTask<SuccessOrError> ChangeBreakoutRooms(
            IServiceMessage<JsonPatchDocument<BreakoutRoomsOptions>> message)
        {
            using var _ = _logger.BeginMethodScope();
            using (await _breakoutLock.LockAsync())
            {
                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                    return CommonError.PermissionDenied(PermissionsList.Rooms.CanCreateAndRemove);

                var current = _synchronizedObject.Current;
                if (current.Active == null)
                {
                    _logger.LogDebug("Breakout rooms must be opened first");
                    return BreakoutRoomError.NotOpen;
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

                var result = new BreakoutRoomsOptionsValidator().Validate(newOptions);
                if (!result.IsValid) return result.ToError();

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

                return SuccessOrError.Succeeded;
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
