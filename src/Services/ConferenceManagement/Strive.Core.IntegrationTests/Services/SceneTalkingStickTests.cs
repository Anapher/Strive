using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using Strive.Core.Domain.Entities;
using Strive.Core.Extensions;
using Strive.Core.IntegrationTests.Services.Base;
using Strive.Core.Interfaces;
using Strive.Core.Interfaces.Services;
using Strive.Core.Services;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.ConferenceControl.Requests;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Permissions.Options;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers.TalkingStick;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Requests;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Scenes.Scenes;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.IntegrationTests.Services
{
    public class SceneTalkingStickTests : ServiceIntegrationTest
    {
        private const string ConferenceId = TestData.ConferenceId;
        private const string DefaultRoomId = RoomOptions.DEFAULT_ROOM_ID;

        public SceneTalkingStickTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesOfNamespace(typeof(CoreModule));
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            builder.RegisterType<TalkingStickModeHandler>().AsImplementedInterfaces().InstancePerDependency();

            var scheduledMediator = new Mock<IScheduledMediator>();
            scheduledMediator.Setup(x => x.Schedule(It.IsAny<IScheduledNotification>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(Guid.NewGuid().ToString());

            builder.RegisterInstance(scheduledMediator.Object).As<IScheduledMediator>();

            builder.RegisterInstance(new OptionsWrapper<RoomOptions>(new RoomOptions())).AsImplementedInterfaces();
            builder.RegisterInstance(new OptionsWrapper<BreakoutRoomsOptions>(new BreakoutRoomsOptions()))
                .AsImplementedInterfaces();
            builder.RegisterInstance(new OptionsWrapper<DefaultPermissionOptions>(new DefaultPermissionOptions()))
                .AsImplementedInterfaces();
            builder.RegisterInstance(new OptionsWrapper<ConcurrencyOptions>(new ConcurrencyOptions()))
                .AsImplementedInterfaces();

            AddConferenceRepo(builder,
                new Conference(ConferenceId) {Configuration = {Moderators = new List<string> {"test"}}});
            SetupConferenceControl(builder);
        }

        [Fact]
        public async Task SetScene_SetToTalkingStick_UpdateSynchronizedObject()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var talkingStickScene = new TalkingStickScene(TalkingStickMode.Queue);

            // act
            await Mediator.Send(new SetSceneRequest(ConferenceId, DefaultRoomId, talkingStickScene));

            // assert
            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(talkingStickScene, scenes.SelectedScene);
            Assert.Equal(new[] {talkingStickScene}, scenes.SceneStack);
        }

        private async Task<Participant> PrepareTalkingStick(TalkingStickMode mode)
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var talkingStickScene = new TalkingStickScene(mode);

            // act
            await Mediator.Send(new SetSceneRequest(ConferenceId, DefaultRoomId, talkingStickScene));

            return TestData.Sven.Participant;
        }

        private void AssertSceneStack(Participant participant, params IScene[] expectedStack)
        {
            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(expectedStack, scenes.SceneStack);
        }

        private static IScene[] SceneStackWithPresenter(TalkingStickMode mode, string presenterId)
        {
            return new IScene[]
            {
                new TalkingStickScene(mode), new PresenterScene(presenterId), new ActiveSpeakerScene(),
            };
        }

        private IReadOnlyDictionary<string, JValue> GetParticipantPermissions(Participant participant)
        {
            var permissions =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedParticipantPermissions>(participant,
                    SynchronizedParticipantPermissions.SyncObjId(participant.Id));

            return permissions.Permissions;
        }

        [Fact]
        public async Task Queue_EnqueueRequest_NoPresenter_MakeParticipantPresenter()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Queue);

            // act
            await Mediator.Send(new TalkingStickEnqueueRequest(participant, false));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Equal(participant.Id, talkingStick.CurrentSpeakerId);
            Assert.Empty(talkingStick.SpeakerQueue);

            AssertSceneStack(participant, SceneStackWithPresenter(TalkingStickMode.Queue, participant.Id));
        }

        [Fact]
        public async Task Queue_EnqueueRequest_HasPresenter_AddToQueue()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Queue);
            await Mediator.Send(new TalkingStickEnqueueRequest(participant, false));

            await JoinParticipant(TestData.Vincent);
            var participant2 = TestData.Vincent.Participant;

            // act
            await Mediator.Send(new TalkingStickEnqueueRequest(participant2, false));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Equal(participant.Id, talkingStick.CurrentSpeakerId);
            Assert.Single(talkingStick.SpeakerQueue, participant2.Id);
        }

        [Fact]
        public async Task Queue_DequeueRequest_HasPresenter_RemoveFromQueue()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Queue);
            await Mediator.Send(new TalkingStickEnqueueRequest(participant, false));

            await JoinParticipant(TestData.Vincent);
            var participant2 = TestData.Vincent.Participant;
            await Mediator.Send(new TalkingStickEnqueueRequest(participant2, false));

            // act
            await Mediator.Send(new TalkingStickEnqueueRequest(participant2, true));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Equal(participant.Id, talkingStick.CurrentSpeakerId);
            Assert.Empty(talkingStick.SpeakerQueue);
        }

        [Fact]
        public async Task Queue_ReturnStick_EmptyQueue_RemovePresenter()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Queue);
            await Mediator.Send(new TalkingStickEnqueueRequest(participant, false));

            // act
            await Mediator.Send(new TalkingStickReturnRequest(participant));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Null(talkingStick.CurrentSpeakerId);
            Assert.Empty(talkingStick.SpeakerQueue);

            AssertSceneStack(participant, new TalkingStickScene(TalkingStickMode.Queue));
        }

        [Fact]
        public async Task Queue_ReturnStick_NonEmptyQueue_SetNewPresenter()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Queue);
            await Mediator.Send(new TalkingStickEnqueueRequest(participant, false));

            await JoinParticipant(TestData.Vincent);
            var participant2 = TestData.Vincent.Participant;
            await Mediator.Send(new TalkingStickEnqueueRequest(participant2, false));

            // act
            await Mediator.Send(new TalkingStickReturnRequest(participant));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Equal(participant2.Id, talkingStick.CurrentSpeakerId);
            Assert.Empty(talkingStick.SpeakerQueue);

            AssertSceneStack(participant, SceneStackWithPresenter(TalkingStickMode.Queue, participant2.Id));
        }

        [Fact]
        public async Task PassTalkingStickRequest_NoPresenter_MakeParticipantPresenter()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Moderated);

            // act
            await Mediator.Send(new TalkingStickPassRequest(participant, DefaultRoomId));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Equal(participant.Id, talkingStick.CurrentSpeakerId);
            Assert.Empty(talkingStick.SpeakerQueue);

            AssertSceneStack(participant, SceneStackWithPresenter(TalkingStickMode.Moderated, participant.Id));
        }

        [Fact]
        public async Task PassTalkingStickRequest_HasPresenter_ReplacePresenter()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Moderated);
            await Mediator.Send(new TalkingStickPassRequest(participant, DefaultRoomId));

            await JoinParticipant(TestData.Vincent);
            var participant2 = TestData.Vincent.Participant;

            // act
            await Mediator.Send(new TalkingStickPassRequest(participant2, DefaultRoomId));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Equal(participant2.Id, talkingStick.CurrentSpeakerId);
            Assert.Empty(talkingStick.SpeakerQueue);

            AssertSceneStack(participant2, SceneStackWithPresenter(TalkingStickMode.Moderated, participant2.Id));
        }

        [Fact]
        public async Task PassTalkingStickRequest_ParticipantWasInQueue_RemoveFromQueue()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Moderated);

            await Mediator.Send(new TalkingStickEnqueueRequest(participant, false));
            await Mediator.Send(new TalkingStickPassRequest(participant, DefaultRoomId));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Empty(talkingStick.SpeakerQueue);
        }

        [Fact]
        public async Task EnqueueRequest_NoQueueMode_StayInQueue()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Moderated);

            // act
            await Mediator.Send(new TalkingStickEnqueueRequest(participant, false));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Single(talkingStick.SpeakerQueue, participant.Id);
        }

        [Fact]
        public async Task ReturnStick_NoQueueMode_RemovePresenter()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Moderated);

            await Mediator.Send(new TalkingStickPassRequest(participant, DefaultRoomId));

            // act
            await Mediator.Send(new TalkingStickReturnRequest(participant));

            // assert
            var talkingStick =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedSceneTalkingStick>(participant,
                    SynchronizedSceneTalkingStick.SyncObjId(DefaultRoomId));

            Assert.Null(talkingStick.CurrentSpeakerId);
            AssertSceneStack(participant, new TalkingStickScene(TalkingStickMode.Moderated));
        }

        [Fact]
        public async Task GetPermissions_IsPresenter_HasPresenterPermissions()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Queue);
            var permissions = GetParticipantPermissions(participant);

            Assert.Contains(permissions,
                x => Equals(x, DefinedPermissions.Scenes.CanQueueForTalkingStick.Configure(true)));

            // act
            await Mediator.Send(new TalkingStickEnqueueRequest(participant, false));

            // assert
            permissions = GetParticipantPermissions(participant);

            Assert.DoesNotContain(permissions,
                x => Equals(x, DefinedPermissions.Scenes.CanQueueForTalkingStick.Configure(true)));
        }

        [Fact]
        public async Task GetPermissions_IsNotPresenter_HasNotPresenterPermissions()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Race);

            await JoinParticipant(TestData.Vincent);
            var participant2 = TestData.Vincent.Participant;
            await Mediator.Send(new TalkingStickPassRequest(participant2, DefaultRoomId));

            var permissions = GetParticipantPermissions(participant);
            Assert.DoesNotContain(permissions,
                x => Equals(x, DefinedPermissions.Scenes.CanTakeTalkingStick.Configure(true)));
        }

        [Fact]
        public async Task GetPermissions_NooneIsPresenter_HasNotPresenterPermissions()
        {
            // arrange
            var participant = await PrepareTalkingStick(TalkingStickMode.Race);
            var permissions =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedParticipantPermissions>(participant,
                    SynchronizedParticipantPermissions.SyncObjId(participant.Id));

            Assert.Contains(permissions.Permissions,
                x => Equals(x, DefinedPermissions.Scenes.CanTakeTalkingStick.Configure(true)));
        }
    }
}
