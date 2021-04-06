using System;
using System.Diagnostics.CodeAnalysis;
using Strive.Core.Services.Chat.Channels;
using Strive.Core.Services.Synchronization;

namespace Strive.Hubs.Core.Validators.Extensions
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
