using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Strive.Core.Services.ConferenceControl.ClientControl;

namespace Strive.Hubs.Core.ClientControl
{
    public class EnableParticipantMessagingRequestHandler : IRequestHandler<EnableParticipantMessagingRequest>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly ILogger<EnableParticipantMessagingRequestHandler> _logger;

        public EnableParticipantMessagingRequestHandler(IHubContext<CoreHub> hubContext,
            ILogger<EnableParticipantMessagingRequestHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(EnableParticipantMessagingRequest request, CancellationToken cancellationToken)
        {
            var (participant, connectionId) = request;

            await _hubContext.Groups.AddToGroupAsync(connectionId, CoreHubGroups.OfParticipant(participant),
                cancellationToken);

            await _hubContext.Groups.AddToGroupAsync(connectionId, CoreHubGroups.OfConference(participant.ConferenceId),
                CancellationToken.None);

            _logger.LogDebug("Added participant {participant} with connection {connectionId}", request.Participant,
                request.ConnectionId);

            return Unit.Value;
        }
    }
}
