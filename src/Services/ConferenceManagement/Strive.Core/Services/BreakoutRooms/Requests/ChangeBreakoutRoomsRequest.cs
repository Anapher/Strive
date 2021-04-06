using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Strive.Core.Interfaces;

namespace Strive.Core.Services.BreakoutRooms.Requests
{
    public record ChangeBreakoutRoomsRequest
        (string ConferenceId, JsonPatchDocument<BreakoutRoomsConfig> Patch) : IRequest<SuccessOrError<Unit>>;
}
