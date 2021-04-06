using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Interfaces.Gateways;

namespace Strive.Core.Services.Chat.Gateways
{
    public interface IChatRepository : IStateRepository
    {
        ValueTask<int> AddChatMessageAndGetMessageCount(string conferenceId, string channel, ChatMessage message);

        ValueTask<EntityPage<ChatMessage>> FetchMessages(string conferenceId, string channel, int start, int end);

        ValueTask DeleteChannel(string conferenceId, string channel);

        ValueTask<IReadOnlyList<string>> FetchAllChannels(string conferenceId);

        ValueTask<bool> AddParticipantTyping(Participant participant, string channel);

        ValueTask<bool> RemoveParticipantTyping(Participant participant, string channel);

        ValueTask<IReadOnlyList<string>> GetAllParticipantsTyping(string conferenceId, string channel);
    }
}
