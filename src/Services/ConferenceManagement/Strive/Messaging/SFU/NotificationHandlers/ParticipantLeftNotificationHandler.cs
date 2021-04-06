using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Notifications;

namespace Strive.Messaging.SFU.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly ISfuNotifier _sfuNotifier;

        public ParticipantLeftNotificationHandler(ISfuNotifier sfuNotifier)
        {
            _sfuNotifier = sfuNotifier;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            await _sfuNotifier.ParticipantLeft(notification.Participant);
        }
    }
}
