using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public record CheckIsParticipantJoinedRequest(Participant Participant) : IRequest<bool>;
}
