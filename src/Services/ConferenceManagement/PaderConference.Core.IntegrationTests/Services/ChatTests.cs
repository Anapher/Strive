using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.IntegrationTests.Services.Base;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Notifications;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.ParticipantsList;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Requests;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.IntegrationTests.Services
{
    public class ChatTests : ServiceIntegrationTest
    {
        private const string ConferenceId = "123";
        private readonly Conference _conference = new(ConferenceId);

        private static readonly Participant TestParticipant1 = new(ConferenceId, "1");
        private static readonly Participant TestParticipant2 = new(ConferenceId, "2");

        private static readonly TestParticipantConnection TestParticipantConnection1 =
            new(TestParticipant1, "test", new ParticipantMetadata("Sven"));

        private static readonly TestParticipantConnection TestParticipantConnection2 =
            new(TestParticipant2, "test2", new ParticipantMetadata("Alfred"));

        private readonly ChatOptions _options = new();

        public ChatTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            SetupConferenceControl(builder);
            AddConferenceRepo(builder, _conference);
            builder.RegisterType<TaskDelay>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ParticipantTypingTimer>().AsImplementedInterfaces().SingleInstance();
            builder.Register(_ => new OptionsWrapper<ChatOptions>(_options)).AsImplementedInterfaces();
            builder.RegisterInstance(new OptionsWrapper<RoomOptions>(new RoomOptions())).AsImplementedInterfaces()
                .SingleInstance();
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesForSynchronizedObjects().Concat(FetchTypesOfNamespace(typeof(SynchronizedChat)))
                .Concat(FetchTypesOfNamespace(typeof(SynchronizedParticipants)))
                .Concat(FetchTypesOfNamespace(typeof(SynchronizedRooms)));
        }

        [Fact]
        public async Task SendChatMessage_SendToGlobalChannel_PublishChatMessageNotification()
        {
            const string message = "Hello World";

            var sender = TestParticipantConnection1;

            // arrange
            await JoinParticipant(TestParticipantConnection1);
            await JoinParticipant(TestParticipantConnection2);

            // act
            var messageOptions = new ChatMessageOptions();
            await Mediator.Send(new SendChatMessageRequest(sender.Participant, message, new GlobalChatChannel(),
                messageOptions));

            // assert
            NotificationCollector.AssertSingleNotificationIssued<ChatMessageReceivedNotification>(notification =>
            {
                Assert.Equal(2, notification.Participants.Count);
                Assert.Contains(TestParticipant1, notification.Participants);
                Assert.Contains(TestParticipant2, notification.Participants);
                Assert.Equal(ConferenceId, notification.ConferenceId);
                Assert.Equal(message, notification.ChatMessage.Message);
                Assert.Equal(sender.Participant.Id, notification.ChatMessage.Sender.ParticipantId);
                Assert.Equal(sender.Meta, notification.ChatMessage.Sender.Meta);
                Assert.Equal(1, notification.TotalMessagesInChannel);
                Assert.Equal(messageOptions, notification.ChatMessage.Options);
            });
        }

        [Fact]
        public async Task SendChatMessage_SendToRoomChannel_OnlyIncludeParticipantsFromTheSameRoom()
        {
            var sender = TestParticipantConnection1;

            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));

            await JoinParticipant(TestParticipantConnection1);
            await JoinParticipant(TestParticipantConnection2);

            var rooms = await Mediator.Send(new CreateRoomsRequest(ConferenceId,
                new[] {new RoomCreationInfo("Room1"), new RoomCreationInfo("Room2")}));

            await Mediator.Send(new SetParticipantRoomRequest(TestParticipant1, rooms[0].RoomId));
            await Mediator.Send(new SetParticipantRoomRequest(TestParticipant2, rooms[1].RoomId));

            // act
            await Mediator.Send(new SendChatMessageRequest(sender.Participant, "Hello World",
                new RoomChatChannel(rooms[0].RoomId), new ChatMessageOptions()));

            // assert
            NotificationCollector.AssertSingleNotificationIssued<ChatMessageReceivedNotification>(notification =>
                {
                    Assert.Equal(TestParticipant1, Assert.Single(notification.Participants));
                });
        }

        [Fact]
        public async Task SetParticipantIsTyping_SetTypingToTrue_UpdateSynchronizedObject()
        {
            // arrange
            await JoinParticipant(TestParticipantConnection1);

            // act
            var channel = new GlobalChatChannel();
            await Mediator.Send(new SetParticipantTypingRequest(TestParticipant1, channel, true));

            // assert
            var syncObjId = SynchronizedChatProvider.GetSyncObjId(channel);
            var synchronizedObject =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedChat>(TestParticipant1, syncObjId);

            var entry = Assert.Single(synchronizedObject.ParticipantsTyping);
            Assert.Equal(entry.Key, TestParticipant1.Id);
        }

        [Fact]
        public async Task SetParticipantIsTyping_SetTypingToBackToFalse_UpdateSynchronizedObject()
        {
            // arrange
            await JoinParticipant(TestParticipantConnection1);

            // act
            var channel = new GlobalChatChannel();
            await Mediator.Send(new SetParticipantTypingRequest(TestParticipant1, channel, true));
            await Mediator.Send(new SetParticipantTypingRequest(TestParticipant1, channel, false));

            // assert
            var syncObjId = SynchronizedChatProvider.GetSyncObjId(channel);
            var synchronizedObject =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedChat>(TestParticipant1, syncObjId);

            Assert.Empty(synchronizedObject.ParticipantsTyping);
        }

        [Fact]
        public async Task FetchMessages_NoMessagesExist_ReturnEmptyList()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));

            // act
            var messages =
                await Mediator.Send(new FetchMessagesRequest(ConferenceId, new GlobalChatChannel(), -50, -1));

            // assert
            Assert.Equal(0, messages.TotalLength);
            Assert.Empty(messages.Result);
        }

        [Fact]
        public async Task FetchMessages_SingleMessageSent_ReturnMessage()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestParticipantConnection1);

            await Mediator.Send(new SendChatMessageRequest(TestParticipant1, "Hello World", new GlobalChatChannel(),
                new ChatMessageOptions()));

            // act
            var messages =
                await Mediator.Send(new FetchMessagesRequest(ConferenceId, new GlobalChatChannel(), -50, -1));

            // assert
            Assert.Equal(1, messages.TotalLength);
            Assert.Single(messages.Result);
        }
    }
}
