using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Strive.Contracts;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Hubs.Core.Responses;

namespace Strive.Hubs.Core.NotificationHandlers
{
    public class ParticipantKickedNotificationHandler : INotificationHandler<ParticipantKickedNotification>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public ParticipantKickedNotificationHandler(IPublishEndpoint publishEndpoint, IHubContext<CoreHub> hubContext)
        {
            _publishEndpoint = publishEndpoint;
            _hubContext = hubContext;
        }

        public async Task Handle(ParticipantKickedNotification notification, CancellationToken cancellationToken)
        {
            var (participant, connectionId, reason) = notification;

            var targetClient = connectionId != null
                ? _hubContext.Clients.Client(connectionId)
                : _hubContext.Clients.Group(CoreHubGroups.OfParticipant(participant));

            await targetClient.OnRequestDisconnect(new RequestDisconnectDto(reason), cancellationToken);
            await _publishEndpoint.Publish<ParticipantKicked>(notification, cancellationToken);
        }
    }
}