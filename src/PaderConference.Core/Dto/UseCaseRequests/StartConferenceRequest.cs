using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class StartConferenceRequest : IUseCaseRequest<StartConferenceResponse>
    {
        public StartConferenceRequest(string userId, ConferenceSettings settings)
        {
            UserId = userId;
            Settings = settings;
        }

        public string UserId { get; }
        public ConferenceSettings Settings { get; }
    }
}