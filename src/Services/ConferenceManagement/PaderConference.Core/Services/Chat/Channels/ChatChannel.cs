namespace PaderConference.Core.Services.Chat.Channels
{
    public abstract record ChatChannel
    {
        public abstract ChatChannelType Type { get; }
    }
}
