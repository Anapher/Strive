using Strive.Core.Services.Poll;

namespace Strive.Hubs.Core.Dtos
{
    public record CreatePollDto(PollInstruction Instruction, PollConfig Config, PollState InitialState, string? RoomId);
}
