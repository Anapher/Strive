using PaderConference.Core.Dto.Services;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class CreateConferenceRequest : IUseCaseRequest<CreateConferenceResponse>
    {
        public CreateConferenceRequest(ConferenceData data, string participantId)
        {
            Data = data;
            ParticipantId = participantId;
        }

        public ConferenceData Data { get; }

        public string ParticipantId { get; }
    }
}
