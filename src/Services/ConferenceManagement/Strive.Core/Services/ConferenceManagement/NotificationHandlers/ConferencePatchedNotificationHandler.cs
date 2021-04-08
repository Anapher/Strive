using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceManagement.Notifications;
using Strive.Core.Services.ParticipantsList;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.ConferenceManagement.NotificationHandlers
{
    // important for chats and moderator synchronized objects
    public class ConferencePatchedNotificationHandler : INotificationHandler<ConferencePatchedNotification>
    {
        private readonly IMediator _mediator;

        public ConferencePatchedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ConferencePatchedNotification notification, CancellationToken cancellationToken)
        {
            var syncParticipants = (SynchronizedParticipants) await _mediator.Send(
                new FetchSynchronizedObjectRequest(notification.ConferenceId, SynchronizedParticipants.SyncObjId));

            foreach (var participantId in syncParticipants.Participants.Keys)
            {
                await _mediator.Send(
                    new UpdateSubscriptionsRequest(new Participant(notification.ConferenceId, participantId)));
            }
        }
    }
}
