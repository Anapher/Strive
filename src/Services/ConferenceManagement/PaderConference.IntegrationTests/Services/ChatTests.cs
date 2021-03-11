using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Hubs;
using PaderConference.Hubs.Dtos;
using PaderConference.Hubs.Responses;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class ChatTests : ServiceIntegrationTest
    {
        public ChatTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper, mongoDb)
        {
        }

        private async Task<SynchronizedObjectId> WaitForGlobalChat(UserConnection connection)
        {
            var chatChannels = await WaitForChatChannels(connection);
            var globalChannelId = ChannelSerializer.Encode(GlobalChatChannel.Instance).ToString();

            return chatChannels.Single(x => x.ToString() == globalChannelId);
        }

        private async Task<IEnumerable<SynchronizedObjectId>> WaitForChatChannels(UserConnection connection)
        {
            var subscriptions = await WaitForSubscriptions(connection);
            return subscriptions.Where(x => x.Id == SynchronizedObjectIds.CHAT);
        }

        private async Task<IEnumerable<SynchronizedObjectId>> WaitForSubscriptions(UserConnection connection)
        {
            var syncSubscriptionsId = SynchronizedSubscriptionsProvider.GetObjIdOfParticipant(connection.User.Sub);

            await connection.SyncObjects.AssertSyncObject<SynchronizedSubscriptions>(syncSubscriptionsId,
                value => Assert.Contains(value.Subscriptions.Keys.Select(SynchronizedObjectId.Parse),
                    x => x.Id == SynchronizedObjectIds.SUBSCRIPTIONS));

            return connection.SyncObjects.GetSynchronizedObject<SynchronizedSubscriptions>(syncSubscriptionsId)
                .Subscriptions.Keys.Select(SynchronizedObjectId.Parse);
        }

        [Fact]
        public async Task Join_ConferenceOpened_NobodyIsTyping()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var channel = await WaitForGlobalChat(connection);
            var syncChat = await connection.SyncObjects.WaitForSyncObj<SynchronizedChat>(channel);

            // assert
            Assert.Empty(syncChat.ParticipantsTyping);
        }

        [Fact]
        public async Task SetTyping_ConferenceOpened_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var channel = await WaitForGlobalChat(connection);

            // act
            await connection.Hub.InvokeAsync(nameof(CoreHub.SetUserIsTyping),
                new SetUserTypingDto(channel.ToString(), true));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedChat>(channel, value =>
            {
                var entry = Assert.Single(value.ParticipantsTyping);
                Assert.Equal(Moderator.Sub, entry.Key);
                Assert.True(entry.Value);
            });
        }

        [Fact]
        public async Task SetNotTyping_ConferenceOpened_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var channel = await WaitForGlobalChat(connection);

            await connection.Hub.InvokeAsync(nameof(CoreHub.SetUserIsTyping),
                new SetUserTypingDto(channel.ToString(), true));
            await connection.SyncObjects.AssertSyncObject<SynchronizedChat>(channel,
                value => Assert.Single(value.ParticipantsTyping));

            // act
            await connection.Hub.InvokeAsync(nameof(CoreHub.SetUserIsTyping),
                new SetUserTypingDto(channel.ToString(), false));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedChat>(channel,
                value => Assert.Empty(value.ParticipantsTyping));
        }

        [Fact]
        public async Task SetTyping_ParticipantNotSubscribedToChat_ReturnError()
        {
            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var roomChannel = await SetupTestRoomChannelAndMoveToRoom(connection);

            var olaf = CreateUser();
            var olafConnection = await ConnectUserToConference(olaf, conference);

            // act
            var result = await olafConnection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetUserIsTyping),
                new SetUserTypingDto(roomChannel, true));

            // assert
            Assert.False(result.Success);
            await connection.SyncObjects.AssertSyncObject<SynchronizedChat>(roomChannel,
                value => Assert.Empty(value.ParticipantsTyping));
        }

        [Fact]
        public async Task SendChatMessageToGlobal_ConferenceOpen_ReceiveNotification()
        {
            const string message = "Hello";

            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var channel = (await WaitForGlobalChat(connection)).ToString();

            var chatMessageNotification = new TaskCompletionSource<ChatMessageDto>();
            connection.Hub.On(CoreHubMessages.Response.ChatMessage,
                (ChatMessageDto dto) => chatMessageNotification.SetResult(dto));

            // act
            await connection.Hub.InvokeAsync(nameof(CoreHub.SendChatMessage),
                new SendChatMessageDto(message, channel, new ChatMessageOptions()));

            // assert
            var messageDto = await chatMessageNotification.Task.WithDefaultTimeout();
            Assert.Equal(message, messageDto.Message);
            Assert.Equal(channel, messageDto.Channel);
            Assert.Equal(1, messageDto.Id);
            Assert.NotNull(messageDto.Sender);
            Assert.Equal(Moderator.Sub, messageDto.Sender!.ParticipantId);
            Assert.Equal(Moderator.Name, messageDto.Sender.Meta.DisplayName);
        }

        [Fact]
        public async Task SendPrivateChatMessage_ParticipantConnected_ReceiveNotificationAndAddToSubscriptions()
        {
            const string message = "Hello";

            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var olaf = CreateUser();
            var olafConnection = await ConnectUserToConference(olaf, conference);

            var channel = ChannelSerializer
                .Encode(new PrivateChatChannel(new HashSet<string> {olaf.Sub, Moderator.Sub})).ToString();

            var chatMessageNotification = new TaskCompletionSource<ChatMessageDto>();
            olafConnection.Hub.On(CoreHubMessages.Response.ChatMessage,
                (ChatMessageDto dto) => chatMessageNotification.SetResult(dto));

            // act
            await connection.Hub.InvokeAsync(nameof(CoreHub.SendChatMessage),
                new SendChatMessageDto(message, channel, new ChatMessageOptions()));

            // assert
            await chatMessageNotification.Task.WithDefaultTimeout();

            var syncSubscriptionsId = SynchronizedSubscriptionsProvider.GetObjIdOfParticipant(connection.User.Sub);
            await connection.SyncObjects.AssertSyncObject<SynchronizedSubscriptions>(syncSubscriptionsId,
                value => Assert.Contains(value.Subscriptions.Keys, x => x == channel));
        }

        private async Task<string> SetupTestRoomChannelAndMoveToRoom(UserConnection connection)
        {
            var subscriptionsId = SynchronizedSubscriptionsProvider.GetObjIdOfParticipant(connection.User.Sub);

            // create room
            var testRoom = (await connection.Hub.InvokeAsync<SuccessOrError<IReadOnlyList<Room>>>(
                nameof(CoreHub.CreateRooms), new List<RoomCreationInfo> {new("TestRoom")})).Response!.Single();

            // switch to room
            await connection.Hub.InvokeAsync(nameof(CoreHub.SwitchRoom), new SwitchRoomDto(testRoom.RoomId));

            // wait until we have subscribed to the room chat
            var testRoomChannel = ChannelSerializer.Encode(new RoomChatChannel(testRoom.RoomId)).ToString();
            await connection.SyncObjects.AssertSyncObject<SynchronizedSubscriptions>(subscriptionsId,
                x => Assert.Contains(x.Subscriptions.Keys, id => id == testRoomChannel));

            return testRoomChannel;
        }

        [Fact]
        public async Task SendChatMessage_ParticipantNotSubscribedToChat_ReturnError()
        {
            const string message = "Hello";

            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var roomChannel = await SetupTestRoomChannelAndMoveToRoom(connection);

            var olaf = CreateUser();
            var olafConnection = await ConnectUserToConference(olaf, conference);

            // act
            var result = await olafConnection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SendChatMessage),
                new SendChatMessageDto(message, roomChannel, new ChatMessageOptions()));

            // assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task FetchChatMessages_ParticipantNotSubscribed_ReturnError()
        {
            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var roomChannel = await SetupTestRoomChannelAndMoveToRoom(connection);

            var olaf = CreateUser();
            var olafConnection = await ConnectUserToConference(olaf, conference);

            // act
            var result =
                await olafConnection.Hub.InvokeAsync<SuccessOrError<IReadOnlyList<ChatMessageDto>>>(
                    nameof(CoreHub.FetchChatMessages), new FetchChatMessagesDto(roomChannel, 0, -1));

            // assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task FetchChatMessages_NoMessagesSent_ReturnEmptyList()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var channel = (await WaitForGlobalChat(connection)).ToString();

            // act
            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<IReadOnlyList<ChatMessageDto>>>(
                    nameof(CoreHub.FetchChatMessages), new FetchChatMessagesDto(channel, 0, -1));

            // assert
            Assert.True(result.Success);
            Assert.Empty(result.Response!);
        }

        [Fact]
        public async Task FetchChatMessages_SomeMessagesSent_ReturnMessages()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var channel = (await WaitForGlobalChat(connection)).ToString();

            Task SendMessage(string message)
            {
                return connection.Hub.InvokeAsync(nameof(CoreHub.SendChatMessage),
                    new SendChatMessageDto(message, channel, new ChatMessageOptions()));
            }

            await SendMessage("Hello");
            await SendMessage("World");
            await SendMessage("Waas geht???");

            // act
            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<IReadOnlyList<ChatMessageDto>>>(
                    nameof(CoreHub.FetchChatMessages), new FetchChatMessagesDto(channel, 0, -1));

            // assert
            Assert.True(result.Success);
            Assert.Equal(3, result.Response!.Count);
        }
    }
}
