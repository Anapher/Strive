using MediatR;

namespace Strive.Core.Services.Poll.Requests
{
    public record UpdateParticipantSubscriptionsOfPollRequest(string ConferenceId, Poll Poll) : IRequest;
}
