using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.ClientControl
{
    public record EnableParticipantMessagingRequest(string ParticipantId, string ConferenceId,
        string ConnectionId) : IRequest<Unit>;
}
