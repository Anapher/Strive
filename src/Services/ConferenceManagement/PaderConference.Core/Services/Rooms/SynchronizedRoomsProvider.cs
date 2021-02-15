using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Rooms
{
    public class SynchronizedRoomsProvider : SynchronizedObjectProvider<SynchronizedRooms>
    {
        private readonly IRoomRepository _roomRepository;

        public SynchronizedRoomsProvider(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public static SynchronizedObjectId SynchronizedObjectId { get; } = new(SynchronizedObjectIds.ROOMS);

        public override string Id { get; } = SynchronizedObjectIds.ROOMS;

        protected override async ValueTask<SynchronizedRooms> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            const string defaultRoomId = RoomOptions.DEFAULT_ROOM_ID;
            var rooms = (await _roomRepository.GetRooms(conferenceId)).OrderBy(x => x.RoomId)
                .ToList(); // maintain default order
            var roomMap = await _roomRepository.GetParticipantRooms(conferenceId);

            return new SynchronizedRooms(rooms, defaultRoomId, roomMap);
        }

        public override ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(string conferenceId,
            string participantId)
        {
            return new(SynchronizedObjectId.Yield());
        }
    }
}
