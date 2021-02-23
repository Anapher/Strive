using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.ParticipantsList.NotificationHandlers
{
    public class ConferenceUpdatedNotificationHandler : INotificationHandler<ConferenceUpdatedNotification>
    {
        private readonly IMediator _mediator;

        public ConferenceUpdatedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(ConferenceUpdatedNotification notification, CancellationToken cancellationToken)
        {
            return _mediator.Send(
                new UpdateSynchronizedObjectRequest(notification.Conference.ConferenceId,
                    SynchronizedParticipantsProvider.SynchronizedObjectId), cancellationToken);
        }
    }
}
