using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Chat.Gateways;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Chat
{
    public class SynchronizedChatProvider : SynchronizedObjectProvider<SynchronizedChat>
    {
        private readonly IMediator _mediator;
        private readonly IChatChannelSelector _chatChannelSelector;
        private readonly IChatRepository _chatRepository;

        public SynchronizedChatProvider(IMediator mediator, IChatChannelSelector chatChannelSelector,
            IChatRepository chatRepository)
        {
            _mediator = mediator;
            _chatChannelSelector = chatChannelSelector;
            _chatRepository = chatRepository;
        }

        public override string Id { get; } = SynchronizedObjectIds.CHAT;

        public override async ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            var result = await _chatChannelSelector.GetAvailableChannels(participant);
            return result.Select(SynchronizedChat.SyncObjId).ToList();
        }

        protected override async ValueTask<SynchronizedChat> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var conference = await _mediator.Send(new FindConferenceByIdRequest(conferenceId));

            if (!conference.Configuration.Chat.ShowTyping)
                return new SynchronizedChat(ImmutableDictionary<string, bool>.Empty);

            var participantsTyping =
                await _chatRepository.GetAllParticipantsTyping(conferenceId, synchronizedObjectId.ToString());

            return new SynchronizedChat(participantsTyping.ToImmutableDictionary(x => x, _ => true));
        }
    }
}
