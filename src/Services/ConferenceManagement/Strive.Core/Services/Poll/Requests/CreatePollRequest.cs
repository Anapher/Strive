using MediatR;

namespace Strive.Core.Services.Poll.Requests
{
    public record CreatePollRequest(string ConferenceId, PollInstruction Instruction, PollConfig Config,
        PollState InitialState, string? RoomId) : IRequest<string>;
}
