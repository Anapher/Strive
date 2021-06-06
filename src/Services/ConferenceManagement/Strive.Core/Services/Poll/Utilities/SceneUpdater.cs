using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Poll.Utilities
{
    public static class SceneUpdater
    {
        public static async Task UpdateScene(IMediator mediator, string conferenceId, string? roomId)
        {
            if (roomId != null)
            {
                await mediator.Send(new UpdateScenesRequest(conferenceId, roomId));
            }
            else
            {
                var rooms = await mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId,
                    SynchronizedRooms.SyncObjId);
                foreach (var room in rooms.Rooms)
                {
                    await mediator.Send(new UpdateScenesRequest(conferenceId, room.RoomId));
                }
            }
        }
    }
}
