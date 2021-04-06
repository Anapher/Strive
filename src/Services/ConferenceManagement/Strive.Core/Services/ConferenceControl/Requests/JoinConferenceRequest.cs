using MediatR;

namespace Strive.Core.Services.ConferenceControl.Requests
{
    public record JoinConferenceRequest
        (Participant Participant, string ConnectionId, ParticipantMetadata Meta) : IRequest;
}
