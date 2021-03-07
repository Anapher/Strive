using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Synchronization.NotificationHandlers
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
                    SynchronizedSubscriptionsProvider.GetObjIdOfParticipant(notification.Participant.Id)),
                cancellationToken);
        }
    }
}
