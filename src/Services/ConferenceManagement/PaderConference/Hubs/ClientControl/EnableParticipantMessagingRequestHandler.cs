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
            var (participant, connectionId) = request;

            await _hubContext.Groups.AddToGroupAsync(connectionId, CoreHubGroups.OfParticipant(participant),
                cancellationToken);

            await _hubContext.Groups.AddToGroupAsync(connectionId, CoreHubGroups.OfConference(participant.ConferenceId),
                CancellationToken.None);

            return Unit.Value;
        }
    }
}
