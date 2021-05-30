using MediatR;

namespace Strive.Core.Services.Poll.Requests
{
    public record UpdatePollStateRequest(string ConferenceId, string PollId, PollState State) : IRequest;
}
