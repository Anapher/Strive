using Strive.Core.Services.Poll;

namespace Strive.Hubs.Core.Dtos
{
    public record UpdatePollStateDto(string PollId, PollState State);
}
