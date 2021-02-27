namespace PaderConference.Core.Services.Chat.Channels
{
    public record GlobalChatChannel : ChatChannel
    {
        public override ChatChannelType Type { get; } = ChatChannelType.Global;
    }
}
