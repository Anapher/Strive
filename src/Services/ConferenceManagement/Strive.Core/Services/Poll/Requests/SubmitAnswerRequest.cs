using MediatR;

namespace Strive.Core.Services.Poll.Requests
{
    public record SubmitAnswerRequest(Participant Participant, string PollId, PollAnswer? Answer) : IRequest;
}
