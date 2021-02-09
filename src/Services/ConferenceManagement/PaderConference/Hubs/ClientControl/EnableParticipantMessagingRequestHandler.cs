using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Services.ConferenceControl.ClientControl;

namespace PaderConference.Hubs.ClientControl
{
    public class EnableParticipantMessagingRequestHandler : IRequestHandler<EnableParticipantMessagingRequest>
    {
        private readonly IHubContext<CoreHub> _hubContext;

        public EnableParticipantMessagingRequestHandler(IHubContext<CoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(EnableParticipantMessagingRequest request, CancellationToken cancellationToken)
        {
            await _hubContext.Groups.AddToGroupAsync(request.ConnectionId,
                CoreHubGroups.OfParticipant(request.ParticipantId), cancellationToken);

            await _hubContext.Groups.AddToGroupAsync(request.ConnectionId,
                CoreHubGroups.OfConference(request.ConferenceId), CancellationToken.None);

            return Unit.Value;
        }
    }
}
