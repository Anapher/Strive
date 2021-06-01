using Strive.Core.Services.Poll;

namespace Strive.Hubs.Core.Dtos
{
    public record SubmitPollAnswerDto(string PollId, PollAnswer Answer);
}
