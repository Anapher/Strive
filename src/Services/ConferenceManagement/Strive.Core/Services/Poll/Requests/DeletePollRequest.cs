using MediatR;

namespace Strive.Core.Services.Poll.Requests
{
    public record DeletePollRequest(string ConferenceId, string PollId) : IRequest;
}
