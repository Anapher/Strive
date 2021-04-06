using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Permissions.Requests;

namespace Strive.Core.Services.Permissions.NotificationHandlers
{
    public class ParticipantInitializedNotificationHandler : INotificationHandler<ParticipantInitializedNotification>
    {
        private readonly IMediator _mediator;

        public ParticipantInitializedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ParticipantInitializedNotification notification, CancellationToken cancellationToken)
        {
            var participant = notification.Participant;
            await _mediator.Send(new UpdateParticipantsPermissionsRequest(participant.Yield()), cancellationToken);
        }
    }
}
