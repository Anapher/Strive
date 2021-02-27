using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.Chat.Channels;

namespace PaderConference.Core.Services.Chat
{
    public interface IChatChannelSelector
    {
        ValueTask<IEnumerable<ChatChannel>> GetAvailableChannels(Participant participant);

        ValueTask<bool> CanParticipantSendMessageToChannel(Participant participant, ChatChannel channel);
    }
}
