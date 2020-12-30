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
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Rooms.Requests;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Rooms
{
    public class RoomsService : ConferenceService, IRoomManagement
    {
        private readonly string _conferenceId;
        private readonly IRoomRepo _roomRepo;
        private readonly ILogger<RoomsService> _logger;
        private readonly RoomOptions _options;
        private readonly IPermissionsService _permissionsService;
        private readonly IConferenceManager _conferenceManager;

        // lock to synchronize deletion and changing of rooms
        private readonly AsyncReaderWriterLock _roomLock = new AsyncReaderWriterLock();
        private readonly ISynchronizedObject<ConferenceRooms> _synchronizedRooms;
        private readonly AsyncLock _autoParticipantLock = new AsyncLock();

        // track the joined participants for conference open/close
        private readonly HashSet<string> _joinedParticipants = new HashSet<string>();
        private bool _isDefaultRoomInitialized;
        private readonly object _defaultRoomLock = new object();

        public RoomsService(string conferenceId, IRoomRepo roomRepo, ISynchronizationManager synchronizationManager,
            IPermissionsService permissionsService, IConferenceManager conferenceManager, IOptions<RoomOptions> options,
            ILogger<RoomsService> logger)
        {
            _conferenceId = conferenceId;
            _roomRepo = roomRepo;
            _permissionsService = permissionsService;
            _conferenceManager = conferenceManager;
            _options = options.Value;
            _logger = logger;

            _synchronizedRooms = synchronizationManager.Register("rooms", new ConferenceRooms());

            permissionsService.RegisterLayerProvider(FetchRoomPermissions);
        }

        public event EventHandler<IReadOnlyList<Room>>? RoomsCreated;
        public event EventHandler<IReadOnlyList<string>>? RoomsRemoved;

        public override ValueTask DisposeAsync()
        {
            _conferenceManager.ConferenceOpened -= ConferenceManagerOnConferenceOpened;
            _conferenceManager.ConferenceClosed -= ConferenceManagerOnConferenceClosed;

            return new ValueTask();
        }

        public override async ValueTask InitializeAsync()
        {
            await DeleteAllRooms();

            _conferenceManager.ConferenceOpened += ConferenceManagerOnConferenceOpened;
            _conferenceManager.ConferenceClosed += ConferenceManagerOnConferenceClosed;

            using (await _autoParticipantLock.LockAsync())
            {
                // if the conference is open. It doesn't matter if ConferenceManagerOnConferenceOpened() was called before,
                // as InitializeDefaultRoom() just ignores the call if it was called before
                var isConferenceOpen = await _conferenceManager.GetIsConferenceOpen(_conferenceId);
                if (isConferenceOpen) await InitializeDefaultRoom();
            }
        }

        /// <summary>
        ///     Create the default room if it is not created yet
        /// </summary>
        private async Task InitializeDefaultRoom()
        {
            if (_isDefaultRoomInitialized) return;

            lock (_defaultRoomLock)
            {
                if (_isDefaultRoomInitialized) return;
                _isDefaultRoomInitialized = true;
            }

            // the default room must always exist
            var defaultRoom = new Room(RoomOptions.DEFAULT_ROOM_ID, _options.DefaultRoomName);
            await _roomRepo.CreateRoom(_conferenceId, defaultRoom);

            await UpdateSynchronizedRooms();
            RoomsCreated?.Invoke(this, new[] {defaultRoom});
        }

        /// <summary>
        ///     Delete all rooms including the default one and remove the participants map
        /// </summary>
        /// <returns></returns>
        private async Task DeleteAllRooms()
        {
            _isDefaultRoomInitialized = false;

            await _roomRepo.DeleteParticipantMapping(_conferenceId);
            await _roomRepo.DeleteAll(_conferenceId);
        }

        public override async ValueTask OnClientConnected(Participant participant)
        {
            using (_logger.BeginScope("OnClientConnected()"))
            using (await _autoParticipantLock.LockAsync())
            {
                _joinedParticipants.Add(participant.ParticipantId);

                if (!await _conferenceManager.GetIsConferenceOpen(_conferenceId))
                {
                    _logger.LogDebug("The conference is not open, do not assign to room.");
                    return;
                }

                await SetRoom(participant.ParticipantId, await GetDefaultRoomId());
            }
        }

        public override async ValueTask OnClientDisconnected(Participant participant)
        {
            using (await _autoParticipantLock.LockAsync())
            {
                _joinedParticipants.Remove(participant.ParticipantId);

                if (!await _conferenceManager.GetIsConferenceOpen(_conferenceId))
                    return;

                await _roomRepo.UnsetParticipantRoom(_conferenceId, participant.ParticipantId);
                await UpdateSynchronizedRooms();
            }
        }

        public async ValueTask<SuccessOrError> SwitchRoom(IServiceMessage<SwitchRoomRequest> message)
        {
            using var _ = _logger.BeginMethodScope();

            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Rooms.CanSwitchRoom))
                return CommonError.PermissionDenied(PermissionsList.Rooms.CanSwitchRoom);

            return await SetRoom(message.Participant.ParticipantId, message.Payload.RoomId);
        }

        public async ValueTask<SuccessOrError> CreateRooms(IServiceMessage<IReadOnlyList<CreateRoomMessage>> message)
        {
            using var _ = _logger.BeginMethodScope();

            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                return CommonError.PermissionDenied(PermissionsList.Rooms.CanCreateAndRemove);

            await CreateRooms(message.Payload);
            return SuccessOrError.Succeeded;
        }

        public async Task<IReadOnlyList<Room>> CreateRooms(IReadOnlyList<CreateRoomMessage> rooms)
        {
            using var _ = _logger.BeginMethodScope();
            _logger.LogDebug("Create {count} room(s)", rooms.Count);

            var result = new List<Room>();
            foreach (var createRoomMessage in rooms)
            {
                var id = Guid.NewGuid().ToString("N");
                var room = new Room(id, createRoomMessage.DisplayName);
                await _roomRepo.CreateRoom(_conferenceId, room);
                result.Add(room);
            }

            await UpdateSynchronizedRooms();
            RoomsCreated?.Invoke(this, result);

            return result;
        }

        public async ValueTask<SuccessOrError> RemoveRooms(IServiceMessage<IReadOnlyList<string>> message)
        {
            using var _ = _logger.BeginMethodScope();

            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Rooms.CanCreateAndRemove))
                return CommonError.PermissionDenied(PermissionsList.Rooms.CanCreateAndRemove);

            await RemoveRooms(message.Payload);
            return SuccessOrError.Succeeded;
        }

        public async Task RemoveRooms(IReadOnlyList<string> roomIds)
        {
            using (await _roomLock.WriterLockAsync())
            {
                var defaultRoomId = await GetDefaultRoomId();
                if (roomIds.Contains(defaultRoomId))
                    throw new InvalidOperationException("Cannot delete default room.");

                var participantToRooms = await _roomRepo.GetParticipantRooms(_conferenceId);
                foreach (var (participantId, roomId) in participantToRooms)
                {
                    if (roomIds.Contains(roomId))
                        await SetRoomInternal(participantId, defaultRoomId);
                }

                await _roomRepo.DeleteRooms(_conferenceId, roomIds);
            }

            await UpdateSynchronizedRooms();
            RoomsRemoved?.Invoke(this, roomIds);
        }

        private async void ConferenceManagerOnConferenceOpened(object? sender, Conference e)
        {
            if (e.ConferenceId != _conferenceId) return;

            // when the conference is opened, we must add all already joined participants to the default room
            using (await _autoParticipantLock.LockAsync())
            {
                await InitializeDefaultRoom();

                foreach (var participant in _joinedParticipants)
                {
                    await SetRoom(participant, await GetDefaultRoomId());
                }
            }
        }

        private async void ConferenceManagerOnConferenceClosed(object? sender, string e)
        {
            if (e != _conferenceId) return;

            using (await _autoParticipantLock.LockAsync())
            {
                await DeleteAllRooms();
            }
        }

        private async Task UpdateSynchronizedRooms()
        {
            var participantToRooms = await _roomRepo.GetParticipantRooms(_conferenceId);
            var defaultRoomId = await GetDefaultRoomId();
            var roomInfos = (await _roomRepo.GetAll(_conferenceId)).Values.OrderBy(x => x.RoomId).ToImmutableList();

            await _synchronizedRooms.Update(new ConferenceRooms
            {
                Rooms = roomInfos,
                DefaultRoomId = defaultRoomId,
                Participants = participantToRooms.ToImmutableDictionary(),
            });
        }

        private async Task<Room?> GetRoomInfo(string roomId)
        {
            return await _roomRepo.Get(_conferenceId, roomId);
        }

        public async Task<SuccessOrError> SetRoom(string participantId, string roomId)
        {
            using (await _roomLock.ReaderLockAsync())
            {
                var result = await SetRoomInternal(participantId, roomId);
                if (!result.Success) return result;
            }

            await UpdateSynchronizedRooms();
            return SuccessOrError.Succeeded;
        }

        private async Task<SuccessOrError> SetRoomInternal(string participantId, string roomId)
        {
            using var _ = _logger.BeginMethodScope(new Dictionary<string, object>
            {
                {"participantId", participantId}, {"roomId", roomId},
            });

            var roomInfo = await GetRoomInfo(roomId);
            if (roomInfo == null)
            {
                _logger.LogDebug("The room is null");
                return RoomsError.RoomNotFound(roomId);
            }

            _logger.LogDebug("Room info received: {@room}", roomInfo);

            _logger.LogDebug("Update room in redis");
            await _roomRepo.SetParticipantRoom(_conferenceId, participantId, roomId);

            return SuccessOrError.Succeeded;
        }

        private async Task<IReadOnlyDictionary<string, JsonElement>?> GetRoomPermissions(string roomId)
        {
            return await _roomRepo.GetRoomPermissions(_conferenceId, roomId);
        }

        private async Task<string> GetDefaultRoomId()
        {
            return await _roomRepo.GetDefaultRoomId(_conferenceId) ?? RoomOptions.DEFAULT_ROOM_ID;
        }

        private async ValueTask<IEnumerable<PermissionLayer>> FetchRoomPermissions(Participant participant)
        {
            var roomId = await _roomRepo.GetParticipantRoom(_conferenceId, participant.ParticipantId);
            if (roomId == null)
                return Enumerable.Empty<PermissionLayer>();

            var roomPermissions = await GetRoomPermissions(roomId);
            if (roomPermissions == null)
                return Enumerable.Empty<PermissionLayer>();

            return CommonPermissionLayers.BreakoutRoom(roomPermissions).Yield();
        }

        public ConferenceRooms State => _synchronizedRooms.Current;
    }
}