using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceManagement.Notifications;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Scenes.NotificationHandlers
{
    public class ConferenceUpdatedNotificationHandler : INotificationHandler<ConferencePatchedNotification>
    {
        private readonly IMediator _mediator;

        public ConferenceUpdatedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ConferencePatchedNotification notification, CancellationToken cancellationToken)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(notification.ConferenceId,
                SynchronizedRooms.SyncObjId);

            foreach (var room in rooms.Rooms)
            {
                await _mediator.Send(new UpdateScenesRequest(notification.ConferenceId, room.RoomId));
            }
        }
    }
}
