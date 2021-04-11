using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Strive.Contracts;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Hubs.Core;

namespace Strive.Messaging.Consumers
{
    public class ParticipantKickedConsumer : IConsumer<ParticipantKicked>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly ICoreHubConnections _connections;
        private readonly IMediator _mediator;
        private readonly IJoinedParticipantsRepository _repository;
        private readonly ILogger<ParticipantKickedConsumer> _logger;

        public ParticipantKickedConsumer(IHubContext<CoreHub> hubContext, ICoreHubConnections connections,
            IMediator mediator, IJoinedParticipantsRepository repository, ILogger<ParticipantKickedConsumer> logger)
        {
            _hubContext = hubContext;
            _connections = connections;
            _mediator = mediator;
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ParticipantKicked> context)
        {
            var message = context.Message;

            await RemoveParticipant(message.Participant, message.ConnectionId, context.CancellationToken);
        }

        private async Task RemoveParticipant(Participant participant, string? connectionId,
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

            if (_connections.TryRemoveParticipant(participant.Id,
                new ParticipantConnection(participant.ConferenceId, connectionId)))
            {
                _logger.LogDebug("Remove participant connection");

                await _hubContext.Groups.RemoveFromGroupAsync(connectionId, CoreHubGroups.OfParticipant(participant),
                    cancellationToken);
                await _hubContext.Groups.RemoveFromGroupAsync(connectionId,
                    CoreHubGroups.OfConference(participant.ConferenceId), cancellationToken);

                if (await _repository.IsParticipantJoined(participant, connectionId))
                {
                    await using var @lock = await _repository.LockParticipantJoin(participant);

                    if (await _repository.IsParticipantJoined(participant, connectionId))
                        await _mediator.Publish(new ParticipantLeftNotification(participant, connectionId),
                            @lock.HandleLostToken);
                }
                else
                {
                    _logger.LogDebug("Participant is not joined");
                }
            }
            else
            {
                _logger.LogDebug("Participant connection was already removed");
            }
        }
    }
}
