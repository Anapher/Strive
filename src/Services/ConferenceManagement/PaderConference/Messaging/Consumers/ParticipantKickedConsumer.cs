using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Contracts;
using PaderConference.Hubs.Core;

namespace PaderConference.Messaging.Consumers
{
    public class ParticipantKickedConsumer : IConsumer<ParticipantKicked>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly ICoreHubConnections _connections;

        public ParticipantKickedConsumer(IHubContext<CoreHub> hubContext, ICoreHubConnections connections)
        {
            _hubContext = hubContext;
            _connections = connections;
        }

        public async Task Consume(ConsumeContext<ParticipantKicked> context)
        {
            var message = context.Message;

            var connectionId = message.ConnectionId;
            if (connectionId == null)
            {
                if (!_connections.TryGetParticipant(message.Participant.Id, out var connection))
                    return;

                connectionId = connection.ConnectionId;
            }

            await _hubContext.Groups.RemoveFromGroupAsync(connectionId,
                CoreHubGroups.OfParticipant(message.Participant), context.CancellationToken);
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId,
                CoreHubGroups.OfConference(message.Participant.ConferenceId), context.CancellationToken);
        }
    }
}
