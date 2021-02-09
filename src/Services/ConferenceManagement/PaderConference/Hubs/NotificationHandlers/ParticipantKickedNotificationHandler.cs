using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Contracts;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Hubs.Responses;

namespace PaderConference.Hubs.NotificationHandlers
{
    public class ParticipantKickedNotificationHandler : INotificationHandler<ParticipantKickedNotification>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly ICoreHubConnections _connections;

        public ParticipantKickedNotificationHandler(IPublishEndpoint publishEndpoint, IHubContext<CoreHub> hubContext,
            ICoreHubConnections connections)
        {
            _publishEndpoint = publishEndpoint;
            _hubContext = hubContext;
            _connections = connections;
        }

        public async Task Handle(ParticipantKickedNotification notification, CancellationToken cancellationToken)
        {
            var (participantId, _, connectionId, reason) = notification;

            var targetClient = connectionId != null
                ? _hubContext.Clients.Client(connectionId)
                : _hubContext.Clients.Group(CoreHubGroups.OfParticipant(participantId));

            await targetClient.OnRequestDisconnect(new RequestDisconnectDto(reason), cancellationToken);
            await _publishEndpoint.Publish<ParticipantKicked>(notification, cancellationToken);
        }
    }
}
