using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Strive.Contracts;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Hubs.Core.Responses;
using Strive.Messaging.Consumers;

namespace Strive.Hubs.Core.NotificationHandlers
{
    public class ParticipantKickedNotificationHandler : INotificationHandler<ParticipantKickedNotification>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly ParticipantKickedConsumer _localConsumer;
        private readonly IPublishEndpoint _publishEndpoint;

        public ParticipantKickedNotificationHandler(IPublishEndpoint publishEndpoint, IHubContext<CoreHub> hubContext,
            ParticipantKickedConsumer localConsumer)
        {
            _publishEndpoint = publishEndpoint;
            _hubContext = hubContext;
            _localConsumer = localConsumer;
        }

        public async Task Handle(ParticipantKickedNotification notification, CancellationToken cancellationToken)
        {
            var (participant, connectionId, reason) = notification;

            var targetClient = connectionId != null
                ? _hubContext.Clients.Client(connectionId)
                : _hubContext.Clients.Group(CoreHubGroups.OfParticipant(participant));

            await targetClient.OnRequestDisconnect(new RequestDisconnectDto(reason), cancellationToken);
            await _publishEndpoint.Publish<ParticipantKicked>(notification, cancellationToken);

            // must be immediately executed if the participant is joined to this server
            await _localConsumer.RemoveParticipant(participant, connectionId, cancellationToken);
        }
    }
}