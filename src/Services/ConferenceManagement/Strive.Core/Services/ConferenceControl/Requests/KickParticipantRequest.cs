using MediatR;

namespace Strive.Core.Services.ConferenceControl.Requests
{
    public record KickParticipantRequest(Participant Participant) : IRequest;
}
