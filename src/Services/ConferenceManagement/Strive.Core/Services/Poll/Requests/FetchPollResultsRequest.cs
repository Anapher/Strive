using MediatR;

namespace Strive.Core.Services.Poll.Requests
{
    public record FetchPollResultsRequest(string ConferenceId, string PollId) : IRequest<SanitizedPollResult>;
}
