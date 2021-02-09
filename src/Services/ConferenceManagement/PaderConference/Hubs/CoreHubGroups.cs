namespace PaderConference.Hubs
{
    public static class CoreHubGroups
    {
        public static string OfConference(string conferenceId)
        {
            return $"Conference:{conferenceId}";
        }

        public static string OfParticipant(string participantId)
        {
            return $"Participant:{participantId}";
        }
    }
}
