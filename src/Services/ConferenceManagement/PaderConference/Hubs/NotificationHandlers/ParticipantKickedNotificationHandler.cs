using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;

namespace PaderConference.Hubs.NotificationHandlers
{
    public class ParticipantKickedNotificationHandler : INotificationHandler<ParticipantKickedNotification>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public ParticipantKickedNotificationHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(ParticipantKickedNotification notification, CancellationToken cancellationToken)
        {
            await _publishEndpoint.Publish(notification, cancellationToken);
        }
    }
}
