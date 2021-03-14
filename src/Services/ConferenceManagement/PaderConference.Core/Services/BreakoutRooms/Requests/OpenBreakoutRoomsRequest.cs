using System;
using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Services.BreakoutRooms.Requests
{
    public record OpenBreakoutRoomsRequest(int Amount, DateTimeOffset? Deadline, string? Description,
        string[][]? AssignedRooms, string ConferenceId) : BreakoutRoomsConfig(Amount, Deadline, Description),
        IRequest<SuccessOrError<Unit>>;
}
