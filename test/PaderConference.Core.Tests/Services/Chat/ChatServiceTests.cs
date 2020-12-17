using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Dto;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Signaling;
using PaderConference.Infrastructure.Sockets;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Chat
{
    public class ChatServiceTests : ServiceTest<ChatService>
    {
        private const string ConferenceId = "C_ID";
        private readonly Mock<ISignalMessenger> _signalMessenger = new Mock<ISignalMessenger>();
        private readonly ChatOptions _options = new ChatOptions();
        private readonly ConnectionMapping _connectionMapping = new ConnectionMapping();
        private readonly MockSynchronizationManager _synchronizationManager = new MockSynchronizationManager();
        private readonly Mock<IConferenceManager> _conferenceManager = new Mock<IConferenceManager>();

        private MockPermissionsService _permissionsService = new MockPermissionsService(
            new Dictionary<string, IReadOnlyDictionary<string, JsonElement>>
            {
                {
                    TestParticipants.Default.ParticipantId,
                    new[] {PermissionsList.Chat.CanSendChatMessage.Configure(true)}.ToDictionary(x => x.Key,
                        x => x.Value)
                },
            });

        public ChatServiceTests(ITestOutputHelper output) : base(output)
        {
        }

        private ChatService Create()
        {
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile<MapperProfile>()));

            return new ChatService(ConferenceId, mapper, _permissionsService, _synchronizationManager,
                _connectionMapping, _signalMessenger.Object, _conferenceManager.Object,
                new OptionsWrapper<ChatOptions>(_options), Logger);
        }

        private ChatSynchronizedObject GetSyncObj()
        {
            return (ChatSynchronizedObject) _synchronizationManager.Objects.Single().Value.GetCurrent();
        }

        #region Send Message

        [Fact]
        public async Task TestSendEmptyMessage()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(new SendChatMessageDto(), TestParticipants.Default, "connectionId");

            // act
            AssertFailed(await service.SendMessage(message.Object));

            // assert
            message.Verify(x => x.SendToCallerAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Never);

            var requestChatMessage = TestServiceMessage.Create(TestParticipants.Default, "connectionId").Object;

            var messages = AssertSuccess(await service.FetchMyMessages(requestChatMessage));
            Assert.Empty(messages);
        }

        [Fact]
        public async Task TestSendMessage()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(new SendChatMessageDto {Message = "Hello world"},
                TestParticipants.Default, "connectionId");

            _signalMessenger
                .Setup(x => x.SendToConferenceAsync(ConferenceId, CoreHubMessages.Response.ChatMessage,
                    It.IsAny<object>())).Callback<string, string, object>((_, __, obj) =>
                {
                    var dto = Assert.IsType<ChatMessageDto>(obj);
                    Assert.Equal("Hello world", dto.Message);
                    Assert.Equal(TestParticipants.Default.ParticipantId, dto.From?.ParticipantId);
                    Assert.Null(dto.Mode);
                });

            // act
            AssertSuccess(await service.SendMessage(message.Object));

            // assert
            _signalMessenger.Verify(
                x => x.SendToConferenceAsync(ConferenceId, CoreHubMessages.Response.ChatMessage,
                    It.IsAny<ChatMessageDto>()), Times.Once);
            _signalMessenger.VerifyNoOtherCalls();

            var requestChatMessage = TestServiceMessage.Create(TestParticipants.Default, "connectionId").Object;

            var result = AssertSuccess(await service.FetchMyMessages(requestChatMessage));
            Assert.Equal("Hello world", Assert.Single(result).Message);
        }

        [Fact]
        public async Task TestSendMessageNoPermissions()
        {
            // arrange
            _permissionsService =
                new MockPermissionsService(new Dictionary<string, IReadOnlyDictionary<string, JsonElement>>());

            var service = Create();
            var message = TestServiceMessage.Create(new SendChatMessageDto {Message = "Hello world"},
                TestParticipants.Default, "connectionId");

            // act
            AssertFailed(await service.SendMessage(message.Object));

            // assert
            message.Verify(x => x.SendToCallerAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Never);

            var requestChatMessage = TestServiceMessage.Create(TestParticipants.Default, "connectionId").Object;
            var messages = AssertSuccess(await service.FetchMyMessages(requestChatMessage));
            Assert.Empty(messages);
        }

        [Fact]
        public async Task TestSendMessageAnonymously()
        {
            // arrange
            _permissionsService = new MockPermissionsService(
                new Dictionary<string, IReadOnlyDictionary<string, JsonElement>>
                {
                    {
                        TestParticipants.Default.ParticipantId,
                        new[]
                        {
                            PermissionsList.Chat.CanSendChatMessage.Configure(true),
                            PermissionsList.Chat.CanSendAnonymousMessage.Configure(true),
                        }.ToDictionary(x => x.Key, x => x.Value)
                    },
                });

            var service = Create();
            var message = TestServiceMessage.Create(
                new SendChatMessageDto {Message = "Hello world", Mode = new SendAnonymously()},
                TestParticipants.Default, "connectionId");

            // act
            AssertSuccess(await service.SendMessage(message.Object));

            // assert
            _signalMessenger.Verify(
                x => x.SendToConferenceAsync(ConferenceId, CoreHubMessages.Response.ChatMessage,
                    It.Is<ChatMessageDto>(x => x.From == null)), Times.Once);
            _signalMessenger.VerifyNoOtherCalls();

            var requestChatMessage = TestServiceMessage.Create(TestParticipants.Default, "connectionId").Object;
            var messages = AssertSuccess(await service.FetchMyMessages(requestChatMessage));

            var existingMessage = Assert.Single(messages);
            Assert.Null(existingMessage.From);
        }

        [Fact]
        public async Task TestSendMessageAnonymouslyWithoutPermissions()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(
                new SendChatMessageDto {Message = "Hello world", Mode = new SendAnonymously()},
                TestParticipants.Default, "connectionId");

            // act
            AssertFailed(await service.SendMessage(message.Object));

            // assert
            _signalMessenger.VerifyNoOtherCalls();
            message.Verify(x => x.SendToCallerAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Never);

            var requestChatMessage = TestServiceMessage.Create(TestParticipants.Default, "connectionId").Object;
            var messages = AssertSuccess(await service.FetchMyMessages(requestChatMessage));
            Assert.Empty(messages);
        }

        #endregion

        #region User Typing

        [Fact]
        public async Task TestSetUserTyping()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(true, TestParticipants.Default, "connectionId");

            // act
            AssertSuccess(await service.SetUserIsTyping(message.Object));

            // assert
            var syncedObj = GetSyncObj();
            var participantTyping = Assert.Single(syncedObj.ParticipantsTyping);
            Assert.Equal(TestParticipants.Default.ParticipantId, participantTyping);
        }

        [Fact]
        public async Task TestSetUserNotTyping()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(false, TestParticipants.Default, "connectionId");

            // act
            AssertSuccess(await service.SetUserIsTyping(message.Object));

            // assert
            var syncedObj = GetSyncObj();
            Assert.Empty(syncedObj.ParticipantsTyping);
        }

        [Fact]
        public async Task TestSetUserTypingAndReset()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(true, TestParticipants.Default, "connectionId");
            var message2 = TestServiceMessage.Create(false, TestParticipants.Default, "connectionId");

            // act
            AssertSuccess(await service.SetUserIsTyping(message.Object));
            Assert.Single(GetSyncObj().ParticipantsTyping);

            AssertSuccess(await service.SetUserIsTyping(message2.Object));

            // assert
            Assert.Empty(GetSyncObj().ParticipantsTyping);
        }

        [Fact]
        public async Task TestSetUserTypingDisconnected()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(true, TestParticipants.Default, "connectionId");

            // act
            AssertSuccess(await service.SetUserIsTyping(message.Object));
            Assert.Single(GetSyncObj().ParticipantsTyping);

            await service.OnClientDisconnected(TestParticipants.Default);

            // assert
            Assert.Empty(GetSyncObj().ParticipantsTyping);
        }

        [Fact]
        public async Task TestSetUserTypingAutoSliding()
        {
            // arrange
            _options.CancelParticipantIsTypingInterval = 0.05;
            _options.CancelParticipantIsTypingAfter = 0.2;

            var service = Create();
            var message = TestServiceMessage.Create(true, TestParticipants.Default, "connectionId");

            // act
            AssertSuccess(await service.SetUserIsTyping(message.Object));
            Assert.Single(GetSyncObj().ParticipantsTyping);

            await Task.Delay(400);

            // assert
            Assert.Empty(GetSyncObj().ParticipantsTyping);
        }

        [Fact]
        public async Task TestSetUserTypingAutoSlidingRefresh()
        {
            // arrange
            _options.CancelParticipantIsTypingInterval = 0.05;
            _options.CancelParticipantIsTypingAfter = 0.2;

            var service = Create();
            var message = TestServiceMessage.Create(true, TestParticipants.Default, "connectionId");

            // act
            AssertSuccess(await service.SetUserIsTyping(message.Object));
            Assert.Single(GetSyncObj().ParticipantsTyping);

            await Task.Delay(150);
            AssertSuccess(await service.SetUserIsTyping(message.Object));

            await Task.Delay(150);
            Assert.Single(GetSyncObj().ParticipantsTyping);

            await Task.Delay(150);

            // assert
            Assert.Empty(GetSyncObj().ParticipantsTyping);
        }

        #endregion

        #region Private Messages

        [Fact]
        public async Task TestSendPrivateMessageWithoutPermissions()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(
                new SendChatMessageDto
                {
                    Message = "Hello", Mode = new SendPrivately {To = new ParticipantRef("test", null)},
                }, TestParticipants.Default, "connectionId");

            var foo = TestParticipants.Default2;
            _conferenceManager.Setup(x => x.TryGetParticipant(It.IsAny<string>(), "test", out foo)).Returns(true);

            // act
            AssertFailed(await service.SendMessage(message.Object));

            // assert
            message.Verify(x => x.SendToCallerAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Never);

            var requestChatMessage = TestServiceMessage.Create(TestParticipants.Default, "connectionId").Object;
            var messages = AssertSuccess(await service.FetchMyMessages(requestChatMessage));
            Assert.Empty(messages);
        }

        [Fact]
        public async Task TestSendPrivateMessage()
        {
            // arrange
            _permissionsService = new MockPermissionsService(
                new Dictionary<string, IReadOnlyDictionary<string, JsonElement>>
                {
                    {
                        TestParticipants.Default.ParticipantId,
                        new[]
                        {
                            PermissionsList.Chat.CanSendChatMessage.Configure(true),
                            PermissionsList.Chat.CanSendPrivateChatMessage.Configure(true),
                        }.ToDictionary(x => x.Key, x => x.Value)
                    },
                });
            _connectionMapping.Add("conn1", TestParticipants.Default, false);
            _connectionMapping.Add("conn2", TestParticipants.Default2, false);
            var foo = TestParticipants.Default2;
            _conferenceManager.Setup(x =>
                    x.TryGetParticipant(It.IsAny<string>(), TestParticipants.Default2.ParticipantId, out foo))
                .Returns(true);

            var service = Create();
            var message = TestServiceMessage.Create(
                new SendChatMessageDto
                {
                    Message = "Hello",
                    Mode = new SendPrivately
                    {
                        To = new ParticipantRef(TestParticipants.Default2.ParticipantId, null),
                    },
                }, TestParticipants.Default, "conn1");

            // act
            AssertSuccess(await service.SendMessage(message.Object));

            // assert
            message.Verify(x => x.SendToCallerAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Never);

            _signalMessenger.Verify(
                x => x.SendToConnectionAsync("conn1", CoreHubMessages.Response.ChatMessage,
                    It.Is<ChatMessageDto>(x => x.Mode is SendPrivately)), Times.Once);
            _signalMessenger.Verify(
                x => x.SendToConnectionAsync("conn2", CoreHubMessages.Response.ChatMessage,
                    It.Is<ChatMessageDto>(x => x.Mode is SendPrivately)), Times.Once);
            _signalMessenger.VerifyNoOtherCalls();

            var requestChatMessage = TestServiceMessage.Create(TestParticipants.Default2, "conn1").Object;
            var messages = AssertSuccess(await service.FetchMyMessages(requestChatMessage));
            Assert.Single(messages);

            requestChatMessage = TestServiceMessage.Create(TestParticipants.Default, "conn1").Object;
            messages = AssertSuccess(await service.FetchMyMessages(requestChatMessage));

            var msg = Assert.Single(messages);
            var participantRef = Assert.IsType<SendPrivately>(msg.Mode).To;
            Assert.NotNull(participantRef);
            Assert.Equal(TestParticipants.Default2.ParticipantId, participantRef.ParticipantId);
            Assert.Equal(TestParticipants.Default2.DisplayName, participantRef.DisplayName);
        }

        #endregion
    }
}
