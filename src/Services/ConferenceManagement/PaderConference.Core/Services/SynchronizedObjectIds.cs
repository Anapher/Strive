namespace PaderConference.Core.Services
{
    public static class SynchronizedObjectIds
    {
        // global/for all
        public const string ROOMS = "rooms";
        public const string CONFERENCE = "conference";

        // individual
        public static string ParticipantPermissions(string participantId)
        {
            return $"participantPermissions:{participantId}";
        }
    }
}
