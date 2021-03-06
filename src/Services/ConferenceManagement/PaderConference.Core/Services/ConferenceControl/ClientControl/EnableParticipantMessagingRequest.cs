using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.ClientControl
{
    public record EnableParticipantMessagingRequest(Participant Participant, string ConnectionId) : IRequest<Unit>;
}
