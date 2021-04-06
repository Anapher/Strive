using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Permissions.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Permissions.NotificationHandlers
{
    public class
        ParticipantPermissionsUpdatedNotificationHandler : INotificationHandler<
            ParticipantPermissionsUpdatedNotification>
    {
        private readonly IMediator _mediator;

        public ParticipantPermissionsUpdatedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ParticipantPermissionsUpdatedNotification notification,
            CancellationToken cancellationToken)
        {
            foreach (var participant in notification.UpdatedPermissions.Keys)
            {
                var syncObjId = SynchronizedParticipantPermissions.SyncObjId(participant.Id);
                await _mediator.Send(new UpdateSynchronizedObjectRequest(participant.ConferenceId, syncObjId),
                    cancellationToken);
            }
        }
    }
}
