using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Rooms.Messages;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Rooms
{
    public class RoomsService : ConferenceService
    {
        private const string DefaultRoomId = "default";
        private readonly string _conferenceId;
        private readonly IRoomRepo _roomRepo;
        private readonly ILogger<RoomsService> _logger;
        private readonly RoomOptions _options;
        private readonly IPermissionsService _permissionsService;
        private readonly AsyncReaderWriterLock _roomLock = new AsyncReaderWriterLock();
        private readonly ISynchronizedObject<ConferenceRooms> _synchronizedRooms;

        public RoomsService(string conferenceId, IRoomRepo roomRepo, ISynchronizationManager synchronizationManager,
            IPermissionsService permissionsService, IOptions<RoomOptions> options, ILogger<RoomsService> logger)
        {
            _conferenceId = conferenceId;
            _roomRepo = roomRepo;
            _permissionsService = permissionsService;
            _options = options.Value;
            _logger = logger;

            _synchronizedRooms = synchronizationManager.Register("rooms",
                new ConferenceRooms(ImmutableList<Room>.Empty, DefaultRoomId,
                    ImmutableDictionary<string, string>.Empty));

            permissionsService.RegisterLayerProvider(FetchRoomPermissions);
        }

        public override async ValueTask OnClientDisconnected(Participant participant)
        {
            await _roomRepo.UnsetParticipantRoom(_conferenceId, participant.ParticipantId);
            await UpdateSynchronizedRooms();
        }

        public override async ValueTask OnClientConnected(Participant participant)
        {
            using (_logger.BeginScope("OnClientConnected()"))
            {
                await SetRoom(participant.ParticipantId, await GetDefaultRoomId());
            }
        }

        public async ValueTask SwitchRoom(IServiceMessage<SwitchRoomMessage> message)
        {
            using (_logger.BeginScope("SwitchRoom()"))
            using (_logger.BeginScope(message.GetScopeData()))
            {
                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!await permissions.GetPermission(PermissionsList.Rooms.CanSwitchRoom))
                {
                    _logger.LogDebug("Permissions to switch room denied.");
                    await message.ResponseError(RoomsError.PermissionToSwitchRoomDenied);
                    return;
                }

                try
                {
                    await SetRoom(message.Participant.ParticipantId, message.Payload.RoomId);
                }
                catch (Exception e)
                {
                    _logger.LogDebug(e, "Switching the room failed");
                    await message.ResponseError(RoomsError.SwitchRoomFailed);
                }
            }
        }

        public async ValueTask CreateRooms(IServiceMessage<IReadOnlyList<CreateRoomMessage>> message)
        {
            using (_logger.BeginScope("CreateRooms()"))
            using (_logger.BeginScope(message.GetScopeData()))
            {
                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                {
                    _logger.LogDebug("Permissions denied.");
                    await message.ResponseError(RoomsError.PermissionToCreateRoomDenied);
                    return;
                }

                foreach (var createRoomMessage in message.Payload)
                {
                    var id = Guid.NewGuid().ToString("N");
                    await _roomRepo.CreateRoom(_conferenceId, new Room(id, createRoomMessage.DisplayName, true));
                }

                await UpdateSynchronizedRooms();
            }
        }

        public async ValueTask RemoveRooms(IServiceMessage<IReadOnlyList<string>> message)
        {
            using (_logger.BeginScope("CreateRooms()"))
            using (_logger.BeginScope(message.GetScopeData()))
            {
                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                {
                    _logger.LogDebug("Permissions denied.");
                    await message.ResponseError(RoomsError.PermissionToRemoveRoomDenied);
                    return;
                }

                using (await _roomLock.WriterLockAsync())
                {
                    var defaultRoomId = await GetDefaultRoomId();
                    if (message.Payload.Contains(defaultRoomId))
                        throw new InvalidOperationException("Cannot delete default room.");

                    var participantToRooms = await _roomRepo.GetParticipantRooms(_conferenceId);
                    foreach (var (participantId, roomId) in participantToRooms)
                        if (message.Payload.Contains(roomId))
                            await SetRoom(participantId, defaultRoomId);

                    await _roomRepo.DeleteRooms(_conferenceId, message.Payload);
                }

                await UpdateSynchronizedRooms();
            }
        }

        public override async ValueTask InitializeAsync()
        {
            await _roomRepo.DeleteAll(_conferenceId);
            await _roomRepo.DeleteParticipantToRoomMap(_conferenceId);

            await _roomRepo.CreateRoom(_conferenceId, new Room(DefaultRoomId, _options.DefaultRoomName, true));

            await UpdateSynchronizedRooms();
        }

        private async Task UpdateSynchronizedRooms()
        {
            var participantToRooms = await _roomRepo.GetParticipantRooms(_conferenceId);
            var defaultRoomId = await GetDefaultRoomId();

            var roomInfos = (await _roomRepo.GetAll(_conferenceId)).Values.OrderBy(x => x.RoomId).ToImmutableList();

            var data = new ConferenceRooms(roomInfos, defaultRoomId,
                participantToRooms?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty);

            await _synchronizedRooms.Update(data);
        }


        private async Task<Room?> GetRoomInfo(string roomId)
        {
            return await _roomRepo.Get(_conferenceId, roomId);
        }

        private async Task SetRoom(string participantId, string roomId)
        {
            using (_logger.BeginScope("SetRoom()"))
            using (_logger.BeginScope(new Dictionary<string, object>
                {{"participantId", participantId}, {"roomId", roomId}}))
            {
                using (await _roomLock.ReaderLockAsync())
                {
                    var roomInfo = await GetRoomInfo(roomId);
                    if (roomInfo == null)
                    {
                        _logger.LogDebug("The room is null");
                        throw new InvalidOperationException("The room does not exist.");
                    }

                    _logger.LogDebug("Room info received: {@room}", roomInfo);

                    if (!roomInfo.IsEnabled)
                    {
                        _logger.LogDebug("The room is disabled");
                        throw new InvalidOperationException("The room is disabled");
                    }

                    _logger.LogDebug("Update room in redis");
                    await _roomRepo.SetParticipantRoom(_conferenceId, participantId, roomId);
                }

                await UpdateSynchronizedRooms();
            }
        }

        private async Task<IReadOnlyDictionary<string, JsonElement>?> GetRoomPermissions(string roomId)
        {
            return await _roomRepo.GetRoomPermissions(_conferenceId, roomId);
        }

        private async Task<string> GetDefaultRoomId()
        {
            return await _roomRepo.GetDefaultRoomId(_conferenceId) ?? DefaultRoomId;
        }

        private async ValueTask<IEnumerable<PermissionLayer>> FetchRoomPermissions(Participant participant)
        {
            var roomId = await _roomRepo.GetParticipantRoom(_conferenceId, participant.ParticipantId);
            if (roomId == null)
                return Enumerable.Empty<PermissionLayer>();

            var roomPermissions = await GetRoomPermissions(roomId);
            if (roomPermissions == null)
                return Enumerable.Empty<PermissionLayer>();

            return new PermissionLayer(10, roomPermissions).Yield();
        }
    }
}