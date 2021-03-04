using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Core.Services.Rooms.Gateways;
using Xunit;

namespace PaderConference.Core.Tests.Services.Chat.Channels
{
    public class ChatChannelSelectorTests
    {
        protected const string RoomId = "123";

        protected static readonly Participant TestParticipant = new("testConferenceId", "testParticipantId");
        private readonly Mock<IChatRepository> _chatRepository = new();

        private readonly ChatChannel _globalChat = GlobalChatChannel.Instance;

        private readonly ChatChannel _privateChat =
            new PrivateChatChannel(new HashSet<string> {TestParticipant.Id, "123"});

        private readonly ChatChannel _roomChat = new RoomChatChannel(RoomId);
        private readonly Mock<IRoomRepository> _roomRepository = new();
        private ChatOptions _chatOptions = new();

        private ChatChannelSelector Create()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(x => x.Send(
                    It.Is<FindConferenceByIdRequest>(x => x.ConferenceId == TestParticipant.ConferenceId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Conference(TestParticipant.ConferenceId)
                    {Configuration = new ConferenceConfiguration() {Chat = _chatOptions}});

            return new ChatChannelSelector(mediatorMock.Object, _roomRepository.Object, _chatRepository.Object);
        }

        [Fact]
        public async Task AllChannelsDeactivated_ParticipantCannotSendAnything()
        {
            // arrange
            _chatOptions = new ChatOptions
            {
                IsPrivateChatEnabled = false, IsGlobalChatEnabled = false, IsRoomChatEnabled = false
            };

            var selector = Create();

            // assert
            await AssertChannels(selector, Array.Empty<ChatChannel>());
        }

        [Fact]
        public async Task GlobalChatActivatedOnly_CanOnlySendToGlobal()
        {
            // arrange
            _chatOptions = new ChatOptions
            {
                IsPrivateChatEnabled = false, IsGlobalChatEnabled = true, IsRoomChatEnabled = false
            };

            var selector = Create();

            // assert
            await AssertChannels(selector, new[] {_globalChat});
        }

        [Fact]
        public async Task GlobalAndRoomActivatedButParticipantNotInRoom_CanOnlySendToGlobal()
        {
            // arrange
            _chatOptions = new ChatOptions
            {
                IsPrivateChatEnabled = false, IsGlobalChatEnabled = true, IsRoomChatEnabled = false
            };

            _roomRepository.Setup(x => x.GetRoomOfParticipant(TestParticipant)).ReturnsAsync((string?) null);

            var selector = Create();

            // assert
            await AssertChannels(selector, new[] {_globalChat});
        }

        [Fact]
        public async Task ParticipantInRoomButRoomChatDisabled_ParticipantCannotSendAnything()
        {
            // arrange
            _chatOptions = new ChatOptions
            {
                IsPrivateChatEnabled = false, IsGlobalChatEnabled = false, IsRoomChatEnabled = false
            };

            _roomRepository.Setup(x => x.GetRoomOfParticipant(TestParticipant)).ReturnsAsync(RoomId);

            var selector = Create();

            // assert
            await AssertChannels(selector, new ChatChannel[0]);
        }

        [Fact]
        public async Task ParticipantInRoomAndRoomChatEnabled_CanSendToChannel()
        {
            // arrange
            _chatOptions = new ChatOptions
            {
                IsPrivateChatEnabled = false, IsGlobalChatEnabled = false, IsRoomChatEnabled = true
            };

            _roomRepository.Setup(x => x.GetRoomOfParticipant(TestParticipant)).ReturnsAsync(RoomId);

            var selector = Create();

            // assert
            await AssertChannels(selector, new[] {_roomChat});
        }

        [Fact]
        public async Task CanParticipantSendMessageToChannel_PrivateChatEnabled_CanSendMessageToAnyoneWithHimInChat()
        {
            // arrange
            _chatOptions = new ChatOptions {IsPrivateChatEnabled = true};

            var selector = Create();

            // assert
            Assert.True(await selector.CanParticipantSendMessageToChannel(TestParticipant, _privateChat));
            Assert.False(await selector.CanParticipantSendMessageToChannel(TestParticipant,
                new PrivateChatChannel(new HashSet<string> {"r1", "r2"})));
            Assert.True(await selector.CanParticipantSendMessageToChannel(TestParticipant,
                new PrivateChatChannel(new HashSet<string> {"r1", TestParticipant.Id})));
        }

        [Fact]
        public async Task GetAvailableChannels_PrivateChatEnabledButNoActiveChats_ReturnEmptyList()
        {
            // arrange
            _chatOptions = new ChatOptions
            {
                IsPrivateChatEnabled = true, IsGlobalChatEnabled = false, IsRoomChatEnabled = false
            };
            _chatRepository.Setup(x => x.FetchAllChannels(TestParticipant.ConferenceId))
                .ReturnsAsync(ImmutableList<string>.Empty);

            var selector = Create();

            // act
            var result = await selector.GetAvailableChannels(TestParticipant);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableChannels_PrivateChatEnabledButAndActiveChats_ReturnChatsWithParticipantOnly()
        {
            // arrange
            _chatOptions = new ChatOptions
            {
                IsPrivateChatEnabled = true, IsGlobalChatEnabled = false, IsRoomChatEnabled = false
            };

            _chatRepository.Setup(x => x.FetchAllChannels(TestParticipant.ConferenceId)).ReturnsAsync(new[]
            {
                "chat?type=global", "chat?type=room&roomId=123",
                $"chat?type=private&p1=test&p2={TestParticipant.Id}", "chat?type=private&p1=t&p2=f"
            });

            var selector = Create();

            // act
            var result = await selector.GetAvailableChannels(TestParticipant);

            // assert
            var chat = Assert.IsType<PrivateChatChannel>(Assert.Single(result));
            Assert.Contains(TestParticipant.Id, chat.Participants);
        }

        private async Task AssertChannels(ChatChannelSelector selector, IEnumerable<ChatChannel> expectedChannels)
        {
            var expectedChannelsList = expectedChannels.ToList();

            await AssertGetAvailableChannels(selector, expectedChannelsList);
            await AssertCanParticipantSendMessageToChannel(selector, expectedChannelsList);
        }

        private async Task AssertGetAvailableChannels(IChatChannelSelector selector,
            IReadOnlyList<ChatChannel> expectedChannels)
        {
            var channels = await selector.GetAvailableChannels(TestParticipant);
            Assert.Equal(expectedChannels, channels);
        }

        private async Task AssertCanParticipantSendMessageToChannel(IChatChannelSelector selector,
            IReadOnlyList<ChatChannel> expectedChannels)
        {
            foreach (var expectedChannel in expectedChannels)
            {
                var result = await selector.CanParticipantSendMessageToChannel(TestParticipant, expectedChannel);
                Assert.True(result);
            }

            var channelsThatShouldNotWork = new List<ChatChannel>();
            if (!expectedChannels.OfType<GlobalChatChannel>().Any())
                channelsThatShouldNotWork.Add(_globalChat);

            if (!expectedChannels.OfType<RoomChatChannel>().Any())
                channelsThatShouldNotWork.Add(_roomChat);

            if (!expectedChannels.OfType<PrivateChatChannel>().Any())
                channelsThatShouldNotWork.Add(_privateChat);

            foreach (var chatChannel in channelsThatShouldNotWork)
            {
                var result = await selector.CanParticipantSendMessageToChannel(TestParticipant, chatChannel);
                Assert.False(result);
            }
        }
    }
}