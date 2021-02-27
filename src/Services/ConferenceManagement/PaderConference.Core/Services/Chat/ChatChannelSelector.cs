using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Chat
{
    public class ChatChannelSelector : IChatChannelSelector
    {
        private readonly ChatOptions _options;
        private readonly IRoomRepository _roomRepository;
        private readonly IChatRepository _chatRepository;

        public ChatChannelSelector(IOptions<ChatOptions> options, IRoomRepository roomRepository,
            IChatRepository chatRepository)
        {
            _options = options.Value;
            _roomRepository = roomRepository;
            _chatRepository = chatRepository;
        }

        public async ValueTask<IEnumerable<ChatChannel>> GetAvailableChannels(Participant participant)
        {
            var result = new List<ChatChannel>();

            if (IsGlobalChatAvailable())
                result.Add(new GlobalChatChannel());

            var roomChatChannel = await GetParticipantRoomChatChannel(participant);
            if (roomChatChannel != null)
                result.Add(roomChatChannel);

            result.AddRange(await FetchPrivateChannels(participant));

            return result;
        }

        public async ValueTask<bool> CanParticipantSendMessageToChannel(Participant participant, ChatChannel channel)
        {
            switch (channel)
            {
                case GlobalChatChannel:
                    return IsGlobalChatAvailable();
                case PrivateChatChannel privateChatChannel:
                    return CanParticipantSendPrivateChatInChannel(participant, privateChatChannel);
                case RoomChatChannel:
                    var participantRoomChatChannel = await GetParticipantRoomChatChannel(participant);
                    return Equals(participantRoomChatChannel, channel);
                default:
                    throw new ArgumentOutOfRangeException(nameof(channel));
            }
        }

        private bool IsGlobalChatAvailable()
        {
            return _options.IsGlobalChatEnabled;
        }

        private async ValueTask<ChatChannel?> GetParticipantRoomChatChannel(Participant participant)
        {
            if (!_options.IsRoomChatEnabled) return null;

            var roomId = await _roomRepository.GetRoomOfParticipant(participant);
            if (roomId != null)
                return new RoomChatChannel(roomId);

            return null;
        }

        private bool CanParticipantSendPrivateChatInChannel(Participant participant,
            PrivateChatChannel privateChatChannel)
        {
            if (!_options.IsPrivateChatEnabled) return false;

            return privateChatChannel.Participants.Contains(participant.Id);
        }

        private async ValueTask<IEnumerable<ChatChannel>> FetchPrivateChannels(Participant participant)
        {
            if (!_options.IsPrivateChatEnabled)
                return Enumerable.Empty<ChatChannel>();

            var allChannels = await _chatRepository.FetchAllChannels(participant.ConferenceId);
            return allChannels.Select(SynchronizedObjectId.Parse).Select(ChannelSerializer.Decode)
                .OfType<PrivateChatChannel>()
                .Where(x => x.Participants.Any(participantId => participantId == participant.Id));
        }
    }
}
