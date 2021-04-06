using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Synchronization.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Synchronization.NotificationHandlers
{
    public class
        UpdateSynchronizedSubscriptionsHandler : INotificationHandler<ParticipantSubscriptionsUpdatedNotification>
    {
        private readonly IMediator _mediator;

        public UpdateSynchronizedSubscriptionsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(ParticipantSubscriptionsUpdatedNotification notification,
            CancellationToken cancellationToken)
        {
            return _mediator.Send(
                new UpdateSynchronizedObjectRequest(notification.Participant.ConferenceId,
                    SynchronizedSubscriptions.SyncObjId(notification.Participant.Id)), cancellationToken);
        }
    }
}
