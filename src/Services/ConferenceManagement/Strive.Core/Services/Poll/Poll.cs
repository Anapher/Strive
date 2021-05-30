namespace Strive.Core.Services.Poll
{
    public record Poll(string Id, PollInstruction Instruction, PollConfig Config, string? RoomId);
}
