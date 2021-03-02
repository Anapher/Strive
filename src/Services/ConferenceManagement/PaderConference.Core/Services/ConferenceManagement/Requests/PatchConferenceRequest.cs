using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using PaderConference.Core.Dto.Services;

namespace PaderConference.Core.Services.ConferenceManagement.Requests
{
    public record PatchConferenceRequest(string ConferenceId, JsonPatchDocument<ConferenceData> Patch) : IRequest;
}
