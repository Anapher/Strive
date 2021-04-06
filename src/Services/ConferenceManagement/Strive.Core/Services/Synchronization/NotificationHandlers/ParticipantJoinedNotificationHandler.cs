using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Synchronization.NotificationHandlers
{
    public class ParticipantJoinedNotificationHandler : INotificationHandler<ParticipantJoinedNotification>
    {
        private readonly IMediator _mediator;

        public ParticipantJoinedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ParticipantJoinedNotification notification, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateSubscriptionsRequest(notification.Participant), cancellationToken);
        }
    }
}
