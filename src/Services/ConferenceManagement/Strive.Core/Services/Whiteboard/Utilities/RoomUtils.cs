using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Whiteboard.Utilities
{
    public static class RoomUtils
    {
        public static async ValueTask<bool> CheckRoomExists(IMediator mediator, string conferenceId, string roomId)
        {
            var syncRooms =
                await mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId, SynchronizedRooms.SyncObjId);

            return syncRooms.Rooms.Any(x => x.RoomId == roomId);
        }
    }
}
