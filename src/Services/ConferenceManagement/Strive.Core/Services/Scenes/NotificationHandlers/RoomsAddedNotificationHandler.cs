using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Scenes.Requests;

namespace Strive.Core.Services.Scenes.NotificationHandlers
{
    public class RoomsAddedNotificationHandler : INotificationHandler<RoomsCreatedNotification>
    {
        private readonly IMediator _mediator;

        public RoomsAddedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(RoomsCreatedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var createdRoomId in notification.CreatedRoomIds)
            {
                await _mediator.Send(new UpdateScenesRequest(notification.ConferenceId, createdRoomId));
            }
        }
    }
}
