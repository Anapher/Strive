using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Dto.UseCaseResponses
{
    public class JoinConferenceResponse
    {
        public JoinConferenceResponse(ParticipantData participantData)
        {
            ParticipantData = participantData;
        }

        public ParticipantData ParticipantData { get; }
    }
}
