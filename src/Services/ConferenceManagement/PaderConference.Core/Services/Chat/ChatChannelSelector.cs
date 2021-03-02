using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Chat
{
    public class ChatChannelSelector : IChatChannelSelector
    {
        private readonly IMediator _mediator;
        private readonly IRoomRepository _roomRepository;
        private readonly IChatRepository _chatRepository;

        public ChatChannelSelector(IMediator mediator, IRoomRepository roomRepository, IChatRepository chatRepository)
        {
            _mediator = mediator;
            _roomRepository = roomRepository;
            _chatRepository = chatRepository;
        }

        public async ValueTask<IEnumerable<ChatChannel>> GetAvailableChannels(Participant participant)
        {
            var result = new List<ChatChannel>();

            var conference = await _mediator.Send(new FindConferenceByIdRequest(participant.ConferenceId));
            var chatOptions = conference.Configuration.Chat;

            if (IsGlobalChatAvailable(chatOptions))
                result.Add(GlobalChatChannel.Instance);

            var roomChatChannel = await GetParticipantRoomChatChannel(chatOptions, participant);
            if (roomChatChannel != null)
                result.Add(roomChatChannel);

            result.AddRange(await FetchPrivateChannels(chatOptions, participant));

            return result;
        }

        public async ValueTask<bool> CanParticipantSendMessageToChannel(Participant participant, ChatChannel channel)
        {
            var conference = await _mediator.Send(new FindConferenceByIdRequest(participant.ConferenceId));
            var chatOptions = conference.Configuration.Chat;

            switch (channel)
            {
                case GlobalChatChannel:
                    return IsGlobalChatAvailable(chatOptions);
                case PrivateChatChannel privateChatChannel:
                    return CanParticipantSendPrivateChatInChannel(chatOptions, participant, privateChatChannel);
                case RoomChatChannel:
                    var participantRoomChatChannel = await GetParticipantRoomChatChannel(chatOptions, participant);
                    return Equals(participantRoomChatChannel, channel);
                default:
                    throw new ArgumentOutOfRangeException(nameof(channel));
            }
        }

        private bool IsGlobalChatAvailable(ChatOptions options)
        {
            return options.IsGlobalChatEnabled;
        }

        private async ValueTask<ChatChannel?> GetParticipantRoomChatChannel(ChatOptions options,
            Participant participant)
        {
            if (!options.IsRoomChatEnabled) return null;

            var roomId = await _roomRepository.GetRoomOfParticipant(participant);
            if (roomId != null)
                return new RoomChatChannel(roomId);

            return null;
        }

        private bool CanParticipantSendPrivateChatInChannel(ChatOptions options, Participant participant,
            PrivateChatChannel privateChatChannel)
        {
            if (!options.IsPrivateChatEnabled) return false;

            return privateChatChannel.Participants.Contains(participant.Id);
        }

        private async ValueTask<IEnumerable<ChatChannel>> FetchPrivateChannels(ChatOptions options,
            Participant participant)
        {
            if (!options.IsPrivateChatEnabled)
                return Enumerable.Empty<ChatChannel>();

            var allChannels = await _chatRepository.FetchAllChannels(participant.ConferenceId);
            return allChannels.Select(SynchronizedObjectId.Parse).Select(ChannelSerializer.Decode)
                .OfType<PrivateChatChannel>()
                .Where(x => x.Participants.Any(participantId => participantId == participant.Id));
        }
    }
}
