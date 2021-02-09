using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.ClientControl
{
    public record DisableParticipantMessagingRequest(string ParticipantId, string ConferenceId,
        string ConnectionId) : IRequest<Unit>;
}
