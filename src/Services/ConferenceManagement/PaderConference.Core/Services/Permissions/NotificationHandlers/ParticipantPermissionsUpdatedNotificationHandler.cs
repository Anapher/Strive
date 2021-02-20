using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Permissions.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Permissions.NotificationHandlers
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
                var syncObjId = SynchronizedParticipantPermissionsProvider.GetObjIdOfParticipant(participant.Id);
                await _mediator.Send(new UpdateSynchronizedObjectRequest(participant.ConferenceId, syncObjId),
                    cancellationToken);
            }
        }
    }
}
