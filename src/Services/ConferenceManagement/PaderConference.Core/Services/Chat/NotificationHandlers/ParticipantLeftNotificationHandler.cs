using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.ConferenceControl.Notifications;

namespace PaderConference.Core.Services.Chat.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IMediator _mediator;
        private readonly IParticipantTypingTimer _participantTypingTimer;

        public ParticipantLeftNotificationHandler(IMediator mediator, IParticipantTypingTimer participantTypingTimer)
        {
            _mediator = mediator;
            _participantTypingTimer = participantTypingTimer;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            var channels = _participantTypingTimer.CancelAllTimersOfParticipant(notification.Participant);
            foreach (var channel in channels)
            {
                await _mediator.Send(new SetParticipantTypingRequest(notification.Participant, channel, false));
            }
        }
    }
}
