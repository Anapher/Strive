using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Strive.Core.Interfaces;

namespace Strive.Core.Services.ConferenceManagement.Requests
{
    public record PatchConferenceRequest
        (string ConferenceId, JsonPatchDocument<ConferenceData> Patch) : IRequest<SuccessOrError<Unit>>;
}
