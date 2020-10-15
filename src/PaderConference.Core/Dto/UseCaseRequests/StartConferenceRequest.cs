using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class StartConferenceRequest : IUseCaseRequest<StartConferenceResponse>
    {
        public StartConferenceRequest(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}