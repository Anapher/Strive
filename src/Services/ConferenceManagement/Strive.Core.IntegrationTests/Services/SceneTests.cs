using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Options;
using Moq;
using Strive.Core.Domain.Entities;
using Strive.Core.Extensions;
using Strive.Core.IntegrationTests.Services.Base;
using Strive.Core.Interfaces;
using Strive.Core.Interfaces.Services;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.BreakoutRooms.Requests;
using Strive.Core.Services.ConferenceControl.Requests;
using Strive.Core.Services.ConferenceManagement;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.Permissions.Options;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers.TalkingStick;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Scenes.Scenes;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.IntegrationTests.Services
{
    public class SceneTests : ServiceIntegrationTest
    {
        private const string ConferenceId = TestData.ConferenceId;
        private const string DefaultRoomId = RoomOptions.DEFAULT_ROOM_ID;

        private static readonly IReadOnlyList<IScene> DefaultSceneStack = new IScene[]
            {AutonomousScene.Instance, GridScene.Instance};

        public SceneTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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
        public async Task FetchAvailableScenesRequest_ConferenceIsOpen_ReturnScenes()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));

            // act
            var result = await Mediator.Send(new FetchAvailableScenesRequest(ConferenceId, DefaultRoomId,
                ImmutableList<IScene>.Empty));

            // assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task FetchAvailableScenesRequest_BreakoutRoomsOpen_ReturnBreakoutRoomScene()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            await Mediator.Send(new OpenBreakoutRoomsRequest(5, null, null, null, ConferenceId));

            // act
            var result = await Mediator.Send(new FetchAvailableScenesRequest(ConferenceId, DefaultRoomId,
                ImmutableList<IScene>.Empty));

            // assert
            Assert.Contains(result, x => Equals(x, BreakoutRoomScene.Instance));
        }

        [Fact]
        public async Task FetchAvailableScenesRequest_TalkingStickInSceneStack_ReturnTalkingStick()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));

            var talkingStickScene = new TalkingStickScene(TalkingStickMode.Queue);

            // act
            var result = await Mediator.Send(new FetchAvailableScenesRequest(ConferenceId, DefaultRoomId,
                new[] {talkingStickScene}));

            // assert
            Assert.Contains(result, x => Equals(x, talkingStickScene));
        }

        [Fact]
        public async Task FetchAvailableScenesRequest_PresenterInSceneStack_ReturnPresenterScene()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var presenterScene = new PresenterScene(TestData.Sven.Participant.Id);

            // act
            var result = await Mediator.Send(new FetchAvailableScenesRequest(ConferenceId, DefaultRoomId,
                new[] {presenterScene}));

            // assert
            Assert.Contains(result, x => Equals(x, presenterScene));
        }

        [Fact]
        public async Task
            FetchAvailableScenesRequest_PresenterInSceneStackButParticipantDoesNotExist_DontReturnPresenterScene()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var presenterScene = new PresenterScene("invalid id");

            // act
            var result = await Mediator.Send(new FetchAvailableScenesRequest(ConferenceId, DefaultRoomId,
                new[] {presenterScene}));

            // assert
            Assert.DoesNotContain(result, x => Equals(x, presenterScene));
        }

        [Fact]
        public async Task JoinRoom_DoNothing_HasSceneStack()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));

            // act
            await JoinParticipant(TestData.Sven);

            // assert
            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(SynchronizedSceneProvider.GetDefaultScene(), scenes.SelectedScene);
            Assert.Equal(DefaultSceneStack, scenes.SceneStack);
            Assert.Null(scenes.OverwrittenContent);
            Assert.NotNull(scenes.AvailableScenes);
        }

        [Fact]
        public async Task SetSceneRequest_ValidScene_UpdateSynchronizedObject()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var scene = GridScene.Instance;

            // act
            await Mediator.Send(new SetSceneRequest(ConferenceId, DefaultRoomId, scene));

            // assert
            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(new[] {scene}, scenes.SceneStack);
            Assert.Equal(scene, scenes.SelectedScene);
        }

        [Fact]
        public async Task SetSceneRequest_InvalidScene_DontChangeScene()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var scene = new ScreenShareScene("some participant");

            // act
            await Mediator.Send(new SetSceneRequest(ConferenceId, DefaultRoomId, scene));

            // assert
            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(DefaultSceneStack, scenes.SceneStack);
        }

        [Fact]
        public async Task SetSceneRequest_InvalidScene_UpdateSynchronizedObject()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var scene = new ScreenShareScene("some participant");

            // act
            await Mediator.Send(new SetSceneRequest(ConferenceId, DefaultRoomId, scene));

            // assert
            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(DefaultSceneStack, scenes.SceneStack);
        }

        [Fact]
        public async Task SetOverwrittenContentSceneRequest_ValidScene_UpdateSynchronizedObject()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var scene = ActiveSpeakerScene.Instance;

            // act
            await Mediator.Send(new SetOverwrittenContentSceneRequest(ConferenceId, DefaultRoomId, scene));

            // assert
            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(DefaultSceneStack.Concat(new[] {ActiveSpeakerScene.Instance}), scenes.SceneStack);
            Assert.Equal(scene, scenes.OverwrittenContent);
        }

        [Fact]
        public async Task SetOverwrittenContentSceneRequest_BreakoutRoomsClosed_RemoveOverwrittenScene()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            await Mediator.Send(new OpenBreakoutRoomsRequest(5, null, null, null, ConferenceId));

            var scene = BreakoutRoomScene.Instance;
            await Mediator.Send(new SetOverwrittenContentSceneRequest(ConferenceId, DefaultRoomId, scene));

            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(DefaultSceneStack.Concat(scene.Yield()), scenes.SceneStack);

            // act
            await Mediator.Send(new CloseBreakoutRoomsRequest(ConferenceId));

            // assert
            scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(DefaultSceneStack, scenes.SceneStack);
            Assert.Equal(AutonomousScene.Instance, scenes.SelectedScene);
            Assert.Null(scenes.OverwrittenContent);
        }

        [Fact]
        public async Task SetSceneRequest_BreakoutRoomsClosed_RevertToOverwrittenScene()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            await Mediator.Send(new OpenBreakoutRoomsRequest(5, null, null, null, ConferenceId));

            await Mediator.Send(new SetSceneRequest(ConferenceId, DefaultRoomId, BreakoutRoomScene.Instance));
            await Mediator.Send(new SetOverwrittenContentSceneRequest(ConferenceId, DefaultRoomId, GridScene.Instance));

            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(new IScene[] {BreakoutRoomScene.Instance, GridScene.Instance}, scenes.SceneStack);

            // act
            await Mediator.Send(new CloseBreakoutRoomsRequest(ConferenceId));

            // assert
            scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(GridScene.Instance, scenes.SelectedScene);
            Assert.Null(scenes.OverwrittenContent);

            Assert.Equal(new IScene[] {GridScene.Instance}, scenes.SceneStack);
        }

        [Fact]
        public async Task ConferenceChanged_DefaultSceneSelected_UpdateSynchronizedObject()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await JoinParticipant(TestData.Sven);

            var scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(DefaultSceneStack, scenes.SceneStack);

            // act
            var patchResult = await Mediator.Send(new PatchConferenceRequest(ConferenceId,
                new JsonPatchDocument<ConferenceData>().Add(x => x.Configuration.Scenes.DefaultScene,
                    SceneOptions.BasicSceneType.ActiveSpeaker)));

            Assert.True(patchResult.Success);

            // assert
            scenes = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedScene>(TestData.Sven.Participant,
                SynchronizedScene.SyncObjId(DefaultRoomId));

            Assert.Equal(new IScene[] {AutonomousScene.Instance, ActiveSpeakerScene.Instance}, scenes.SceneStack);
        }
    }
}
