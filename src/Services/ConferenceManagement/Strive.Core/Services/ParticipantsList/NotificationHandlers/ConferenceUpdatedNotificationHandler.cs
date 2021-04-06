using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.ParticipantsList.NotificationHandlers
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
                    SynchronizedParticipants.SyncObjId), cancellationToken);
        }
    }
}
