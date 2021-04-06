using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Services.Chat.Channels;

namespace Strive.Core.Services.Chat
{
    public interface IChatChannelSelector
    {
        ValueTask<IEnumerable<ChatChannel>> GetAvailableChannels(Participant participant);

        ValueTask<bool> CanParticipantSendMessageToChannel(Participant participant, ChatChannel channel);
    }
}
