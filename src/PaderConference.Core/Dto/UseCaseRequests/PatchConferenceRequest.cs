using Microsoft.AspNetCore.JsonPatch;
using PaderConference.Core.Dto.Services;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class PatchConferenceRequest : IUseCaseRequest<PatchConferenceResponse>
    {
        public PatchConferenceRequest(JsonPatchDocument<ConferenceData> patch, string conferenceId)
        {
            Patch = patch;
            ConferenceId = conferenceId;
        }

        public string ConferenceId { get; }

        public JsonPatchDocument<ConferenceData> Patch { get; }
    }
}
