using PaderConference.Core.Services;

namespace PaderConference.Hubs.Core
{
    public static class CoreHubGroups
    {
        public static string OfConference(string conferenceId)
        {
            return $"Conference:{conferenceId}";
        }

        public static string OfParticipant(Participant participant)
        {
            return $"Participant:{participant.ConferenceId}/{participant.Id}";
        }
    }
}
