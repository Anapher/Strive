using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Dto.UseCaseResponses
{
    public class JoinConferenceResponse
    {
        public JoinConferenceResponse(Participant participant)
        {
            Participant = participant;
        }

        public Participant Participant { get; }
    }
}
