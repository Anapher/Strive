using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Services.ConferenceControl.ClientControl;

namespace PaderConference.Hubs.ClientControl
{
    public class DisableParticipantMessagingRequestHandler : IRequestHandler<DisableParticipantMessagingRequest>
    {
        private readonly IHubContext<CoreHub> _hubContext;

        public DisableParticipantMessagingRequestHandler(IHubContext<CoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(DisableParticipantMessagingRequest request, CancellationToken cancellationToken)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(request.ConnectionId,
                CoreHubGroups.OfParticipant(request.ParticipantId), cancellationToken);

            await _hubContext.Groups.RemoveFromGroupAsync(request.ConnectionId,
                CoreHubGroups.OfConference(request.ConferenceId), CancellationToken.None);

            return Unit.Value;
        }
    }
}
