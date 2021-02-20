using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public record JoinConferenceRequest (Participant Participant, string ConnectionId) : IRequest;
}
