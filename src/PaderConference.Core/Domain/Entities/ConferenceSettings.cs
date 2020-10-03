namespace PaderConference.Core.Domain.Entities
{
    public class ConferenceSettings
    {
        public bool AllowUsersToUnmute { get; set; }

        public ChatOptions Chat { get; set; } = new ChatOptions();
    }
}