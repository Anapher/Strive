namespace Strive.Core.Services.Chat.Channels
{
    public record GlobalChatChannel : ChatChannel
    {
        public static GlobalChatChannel Instance { get; } = new();
    }
}
