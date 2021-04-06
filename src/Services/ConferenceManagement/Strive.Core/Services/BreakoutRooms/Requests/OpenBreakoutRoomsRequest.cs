using System;
using MediatR;
using Strive.Core.Interfaces;

namespace Strive.Core.Services.BreakoutRooms.Requests
{
    public record OpenBreakoutRoomsRequest(int Amount, DateTimeOffset? Deadline, string? Description,
        string[][]? AssignedRooms, string ConferenceId) : BreakoutRoomsConfig(Amount, Deadline, Description),
        IRequest<SuccessOrError<Unit>>;
}
