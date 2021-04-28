using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceManagement.Notifications;
using Strive.Core.Services.ParticipantsList;
using Strive.Core.Services.Permissions.Requests;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Permissions.NotificationHandlers
{
    public class ConferencePatchedNotificationHandler : INotificationHandler<ConferencePatchedNotification>
    {
        private readonly IMediator _mediator;

        public ConferencePatchedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ConferencePatchedNotification notification, CancellationToken cancellationToken)
        {
            var syncParticipants = await _mediator.FetchSynchronizedObject<SynchronizedParticipants>(
                notification.ConferenceId, SynchronizedParticipants.SyncObjId);

            await _mediator.Send(new UpdateParticipantsPermissionsRequest(
                syncParticipants.Participants.Keys.Select(x => new Participant(notification.ConferenceId, x))));
        }
    }
}
