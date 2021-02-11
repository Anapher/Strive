using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Rooms
{
    public class SynchronizedRoomsProvider : SynchronizedObjectProvider<SynchronizedRooms>
    {
        private readonly IRoomRepository _roomRepository;

        public SynchronizedRoomsProvider(IMediator mediator, IRoomRepository roomRepository) : base(mediator)
        {
            _roomRepository = roomRepository;
        }

        public override async ValueTask<SynchronizedRooms> GetInitialValue(string conferenceId)
        {
            const string defaultRoomId = RoomOptions.DEFAULT_ROOM_ID;
            var rooms = (await _roomRepository.GetRooms(conferenceId)).OrderBy(x => x.RoomId)
                .ToList(); // maintain default order
            var roomMap = await _roomRepository.GetParticipantRooms(conferenceId);

            return new SynchronizedRooms(rooms, defaultRoomId, roomMap);
        }
    }
}
