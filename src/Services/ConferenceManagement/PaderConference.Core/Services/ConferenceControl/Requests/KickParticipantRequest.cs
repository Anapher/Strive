using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public record KickParticipantRequest(string ParticipantId, string ConferenceId) : IRequest;
}
