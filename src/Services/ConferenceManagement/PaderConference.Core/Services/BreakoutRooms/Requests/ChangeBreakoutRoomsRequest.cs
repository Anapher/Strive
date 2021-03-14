using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Services.BreakoutRooms.Requests
{
    public record ChangeBreakoutRoomsRequest
        (string ConferenceId, JsonPatchDocument<BreakoutRoomsConfig> Patch) : IRequest<SuccessOrError<Unit>>;
}
