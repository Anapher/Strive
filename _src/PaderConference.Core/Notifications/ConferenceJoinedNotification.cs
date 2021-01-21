using MediatR;

namespace PaderConference.Core.Notifications
{
    public class ConferenceJoinedNotification : INotification
    {
        public ConferenceJoinedNotification(string conferenceId, string participantId)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
        }

        public string ConferenceId { get; }
        public string ParticipantId { get; }
    }
}
