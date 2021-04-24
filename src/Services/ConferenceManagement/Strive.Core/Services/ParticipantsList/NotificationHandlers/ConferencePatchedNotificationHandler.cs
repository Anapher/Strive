using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceManagement.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.ParticipantsList.NotificationHandlers
{
    public class ConferencePatchedNotificationHandler : INotificationHandler<ConferencePatchedNotification>
    {
        private readonly IMediator _mediator;

        public ConferencePatchedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(ConferencePatchedNotification notification, CancellationToken cancellationToken)
        {
            return _mediator.Send(
                new UpdateSynchronizedObjectRequest(notification.ConferenceId, SynchronizedParticipants.SyncObjId),
                cancellationToken);
        }
    }
}
