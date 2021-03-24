using System;
using System.Diagnostics.CodeAnalysis;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Hubs.Core.Validators.Extensions
{
    public static class ChatValidationExtensions
    {
        public static bool TryParseChatChannel(string s, [NotNullWhen(true)] out ChatChannel? channel)
        {
            try
            {
                channel = ChannelSerializer.Decode(SynchronizedObjectId.Parse(s));
                return true;
            }
            catch (Exception)
            {
                channel = null;
                return false;
            }
        }
    }
}
