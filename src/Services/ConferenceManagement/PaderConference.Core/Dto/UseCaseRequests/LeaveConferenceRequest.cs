using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class LeaveConferenceRequest : IUseCaseRequest<LeaveConferenceResponse>
    {
        public LeaveConferenceRequest(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; }
    }
}
