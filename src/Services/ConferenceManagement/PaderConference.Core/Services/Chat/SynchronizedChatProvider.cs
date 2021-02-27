using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Chat
{
    public class SynchronizedChatProvider : SynchronizedObjectProvider<SynchronizedChat>
    {
        private readonly IChatChannelSelector _chatChannelSelector;
        private readonly IChatRepository _chatRepository;
        private readonly ChatOptions _options;

        public SynchronizedChatProvider(IChatChannelSelector chatChannelSelector, IChatRepository chatRepository,
            IOptions<ChatOptions> options)
        {
            _chatChannelSelector = chatChannelSelector;
            _chatRepository = chatRepository;
            _options = options.Value;
        }

        public override string Id { get; } = SynchronizedObjectIds.CHAT;

        public override async ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            var result = await _chatChannelSelector.GetAvailableChannels(participant);
            return result.Select(ChannelSerializer.Encode).ToList();
        }

        protected override async ValueTask<SynchronizedChat> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            if (!_options.ShowTyping)
                return new SynchronizedChat(ImmutableDictionary<string, bool>.Empty);

            var participantsTyping =
                await _chatRepository.GetAllParticipantsTyping(conferenceId, synchronizedObjectId.ToString());

            return new SynchronizedChat(participantsTyping.ToImmutableDictionary(x => x, _ => true));
        }
    }
}
