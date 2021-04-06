using System.Collections.Generic;
using Strive.Core.Services.Chat.Channels;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Chat
{
    public record SynchronizedChat(IReadOnlyDictionary<string, bool> ParticipantsTyping)
    {
        public static SynchronizedObjectId SyncObjId(ChatChannel channel)
        {
            return ChannelSerializer.Encode(channel);
        }
    }
}
