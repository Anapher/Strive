using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public record KickParticipantRequest(Participant Participant) : IRequest;
}
