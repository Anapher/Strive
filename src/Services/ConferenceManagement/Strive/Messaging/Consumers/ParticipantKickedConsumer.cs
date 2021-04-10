using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Strive.Contracts;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Hubs.Core;

namespace Strive.Messaging.Consumers
{
    public class ParticipantKickedConsumer : IConsumer<ParticipantKicked>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly ICoreHubConnections _connections;
        private readonly IMediator _mediator;
        private readonly ILogger<ParticipantKickedConsumer> _logger;

        public ParticipantKickedConsumer(IHubContext<CoreHub> hubContext, ICoreHubConnections connections,
            IMediator mediator, ILogger<ParticipantKickedConsumer> logger)
        {
            _hubContext = hubContext;
            _connections = connections;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ParticipantKicked> context)
        {
            var message = context.Message;

            await RemoveParticipant(message.Participant, message.ConnectionId, context.CancellationToken);
        }

        public async Task RemoveParticipant(Participant participant, string? connectionId,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("RemoveParticipant() | {participant}, connectionId:{connectionId}", participant,
                connectionId);

            if (connectionId == null)
            {
                if (!_connections.TryGetParticipant(participant.Id, out var connection))
                    return;

                connectionId = connection.ConnectionId;
            }

            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, CoreHubGroups.OfParticipant(participant),
                cancellationToken);
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId,
                CoreHubGroups.OfConference(participant.ConferenceId), cancellationToken);

            await _mediator.Publish(new ParticipantLeftNotification(participant, connectionId));
        }
    }
}
