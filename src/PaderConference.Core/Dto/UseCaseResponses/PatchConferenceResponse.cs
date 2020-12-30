using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Dto.UseCaseResponses
{
    public class PatchConferenceResponse
    {
        public PatchConferenceResponse(Conference conference)
        {
            Conference = conference;
        }

        public Conference Conference { get; }
    }
}
