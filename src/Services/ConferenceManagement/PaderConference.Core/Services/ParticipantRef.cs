using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services
{
    public class ParticipantRef
    {
        public ParticipantRef(string participantId, string? displayName)
        {
            ParticipantId = participantId;
            DisplayName = displayName;
        }

        public ParticipantRef()
        {
        }

        public string? ParticipantId { get; set; }

        public string? DisplayName { get; set; }

        public static ParticipantRef FromParticipant(Participant participant)
        {
            return new ParticipantRef(participant.ParticipantId, participant.DisplayName);
        }
    }
}
