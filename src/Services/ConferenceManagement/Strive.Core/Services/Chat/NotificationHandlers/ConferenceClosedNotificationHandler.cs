using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Notifications;

namespace Strive.Core.Services.Chat.NotificationHandlers
{
    public class ConferenceClosedNotificationHandler : INotificationHandler<ConferenceClosedNotification>
    {
        private readonly IParticipantTypingTimer _participantTypingTimer;

        public ConferenceClosedNotificationHandler(IParticipantTypingTimer participantTypingTimer)
        {
            _participantTypingTimer = participantTypingTimer;
        }

        public async Task Handle(ConferenceClosedNotification notification, CancellationToken cancellationToken)
        {
            _participantTypingTimer.CancelAllTimersOfConference(notification.ConferenceId);
        }
    }
}
