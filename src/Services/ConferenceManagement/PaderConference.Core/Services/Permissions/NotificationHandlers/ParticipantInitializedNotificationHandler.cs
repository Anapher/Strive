using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Permissions.Requests;

namespace PaderConference.Core.Services.Permissions.NotificationHandlers
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
            var (participantId, conferenceId) = notification;
            await _mediator.Send(new UpdateParticipantsPermissionsRequest(conferenceId, participantId.Yield()),
                cancellationToken);
        }
    }
}
