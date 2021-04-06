using MediatR;

namespace Strive.Core.Services.ConferenceControl.Requests
{
    public record CheckIsParticipantJoinedRequest(Participant Participant) : IRequest<bool>;
}
