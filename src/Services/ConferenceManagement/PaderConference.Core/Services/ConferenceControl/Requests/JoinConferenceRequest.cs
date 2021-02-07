using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public record JoinConferenceRequest
        (string ConferenceId, string ParticipantId, string Role, string? Name) : IRequest;
}
