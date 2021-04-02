using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Rooms
{
    public class SynchronizedRoomsProvider : SynchronizedObjectProviderForAll<SynchronizedRooms>
    {
        private readonly IRoomRepository _roomRepository;

        public SynchronizedRoomsProvider(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public override string Id { get; } = SynchronizedRooms.SyncObjId.Id;

        protected override async ValueTask<SynchronizedRooms> InternalFetchValue(string conferenceId)
        {
            const string defaultRoomId = RoomOptions.DEFAULT_ROOM_ID;
            var rooms = (await _roomRepository.GetRooms(conferenceId)).OrderBy(x => x.RoomId)
                .ToList(); // maintain default order
            var roomMap = await _roomRepository.GetParticipantRooms(conferenceId);

            return new SynchronizedRooms(rooms, defaultRoomId, roomMap);
        }
    }
}
