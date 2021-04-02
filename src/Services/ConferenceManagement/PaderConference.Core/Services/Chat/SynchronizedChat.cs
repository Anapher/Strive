using System.Collections.Generic;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Chat
{
    public record SynchronizedChat(IReadOnlyDictionary<string, bool> ParticipantsTyping)
    {
        public static SynchronizedObjectId SyncObjId(ChatChannel channel)
        {
            return ChannelSerializer.Encode(channel);
        }
    }
}
