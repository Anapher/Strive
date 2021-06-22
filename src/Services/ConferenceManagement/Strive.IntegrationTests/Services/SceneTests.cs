using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Strive.Core;
using Strive.Core.Extensions;
using Strive.Core.Interfaces;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers.TalkingStick;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Hubs.Core;
using Strive.Hubs.Core.Dtos;
using Strive.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class SceneTests : ServiceIntegrationTest
    {
        private static readonly IReadOnlyList<IScene> DefaultSceneStack = new IScene[]
            {AutonomousScene.Instance, GridScene.Instance};

        public SceneTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper, mongoDb)
        {
        }

        [Fact]
        public async Task SetScene_RoomDoesNotExist_ReturnError()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var conn = await ConnectUserToConference(Moderator, conference);

            // act
            var result = await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene), GridScene.Instance);

            // assert
            AssertFailed(result);
            Assert.Equal(ConferenceError.RoomNotFound.Code, result.Error?.Code);
        }

        [Fact]
        public async Task JoinToRoom_DoNothing_SynchronizeAvailableScenes()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.NotEmpty(syncObj.AvailableScenes);
                    Assert.Equal(AutonomousScene.Instance, syncObj.SelectedScene);
                    Assert.Equal(DefaultSceneStack, syncObj.SceneStack);
                });
        }

        [Fact]
        public async Task SetScene_SceneIsNull_ReturnError()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // act
            var result = await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene), null);

            // assert
            AssertFailed(result);
        }

        [Fact]
        public async Task SetScene_SceneIsAvailable_UpdateSynchronizedObject()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // act
            var result = await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene), GridScene.Instance);

            // assert
            AssertSuccess(result);

            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(GridScene.Instance, syncObj.SelectedScene);
                    Assert.Single(syncObj.SceneStack, GridScene.Instance);
                });
        }

        [Fact]
        public async Task SetScene_SceneIsNotAvailable_DontChangeCurrentScene()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            var current =
                await conn.SyncObjects.WaitForSyncObj<SynchronizedScene>(
                    SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID));

            // act
            await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene), new ScreenShareScene("5342"));

            // assert

            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => Assert.Equal(current.SceneStack, syncObj.SceneStack));
        }

        [Fact]
        public async Task SetScene_MakePresenter_UpdateSceneStack()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // act
            await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new PresenterScene(conn.User.Sub));

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => Assert.Equal(new IScene[] {new PresenterScene(conn.User.Sub), ActiveSpeakerScene.Instance},
                    syncObj.SceneStack));
        }

        [Fact]
        public async Task SetOverwrittenScene_SceneIsAvailable_UpdateSynchronizedObject()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // act
            var result =
                await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetOverwrittenScene),
                    GridScene.Instance);

            // assert
            AssertSuccess(result);

            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(GridScene.Instance, syncObj.OverwrittenContent);
                    Assert.Equal(syncObj.SceneStack, DefaultSceneStack.Concat(GridScene.Instance.Yield()));
                });
        }

        [Fact]
        public async Task SetOverwrittenScene_SceneIsNotAvailable_DontSetScene()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // act
            var result = await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetOverwrittenScene),
                new ScreenShareScene("1235"));

            // assert
            AssertSuccess(result);

            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Null(syncObj.OverwrittenContent);
                    Assert.Equal(syncObj.SceneStack, DefaultSceneStack);
                });
        }

        [Fact]
        public async Task OpenBreakoutRooms_Joined_BreakoutRoomSceneBecomesAvailable()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            await conn.SyncObjects.AssertSyncObject(SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                (SynchronizedScene syncObj) =>
                    Assert.DoesNotContain(syncObj.AvailableScenes, x => x is BreakoutRoomScene));

            // act
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
                new OpenBreakoutRoomsDto(5, null, null, null)));

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => Assert.Contains(syncObj.AvailableScenes, x => x is BreakoutRoomScene));
        }

        [Fact]
        public async Task CloseBreakoutRooms_Joined_BreakoutRoomSceneBecomesUnavailable()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // act
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
                new OpenBreakoutRoomsDto(5, null, null, null)));

            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => Assert.Contains(syncObj.AvailableScenes, x => x is BreakoutRoomScene));

            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseBreakoutRooms)));

            // assert
            await conn.SyncObjects.AssertSyncObject(SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                (SynchronizedScene syncObj) =>
                    Assert.DoesNotContain(syncObj.AvailableScenes, x => x is BreakoutRoomScene));
        }

        [Fact]
        public async Task CloseBreakoutRooms_BreakoutRoomIsCurrentScene_RemoveCurrentScene()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // act
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
                new OpenBreakoutRoomsDto(5, null, null, null)));

            AssertSuccess(
                await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene), BreakoutRoomScene.Instance));

            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => Assert.Equal(BreakoutRoomScene.Instance, syncObj.SelectedScene));

            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseBreakoutRooms)));

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => Assert.Equal(AutonomousScene.Instance, syncObj.SelectedScene));
        }

        [Fact]
        public async Task TalkingStick_Race_ParticipantsTakeStick_MakePresenter()
        {
            // arrange
            var (conn, conference) = await ConnectToOpenedConference();
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new TalkingStickScene(TalkingStickMode.Race)));

            var pleb = CreateUser();
            var plebConn = await ConnectUserToConference(pleb, conference);

            // act
            AssertSuccess(await plebConn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.TalkingStickTake)));

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(syncObj.SceneStack, new IScene[]
                    {
                        new TalkingStickScene(TalkingStickMode.Race), new PresenterScene(pleb.Sub),
                        new ActiveSpeakerScene(),
                    });
                });

            await plebConn.SyncObjects.AssertSyncObject<SynchronizedParticipantPermissions>(
                SynchronizedParticipantPermissions.SyncObjId(pleb.Sub),
                permissions =>
                {
                    Assert.Contains(permissions.Permissions,
                        x => Equals(x, DefinedPermissions.Scenes.CanOverwriteContentScene.Configure(true)));
                });
        }

        [Fact]
        public async Task TalkingStick_Race_PresenterLeaves_RemoveScene()
        {
            // arrange
            var (conn, conference) = await ConnectToOpenedConference();
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new TalkingStickScene(TalkingStickMode.Race)));

            var pleb = CreateUser();
            var plebConn = await ConnectUserToConference(pleb, conference);

            AssertSuccess(await plebConn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.TalkingStickTake)));
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(syncObj.SceneStack, new IScene[]
                    {
                        new TalkingStickScene(TalkingStickMode.Race), new PresenterScene(pleb.Sub),
                        new ActiveSpeakerScene(),
                    });
                });

            // act
            await plebConn.Hub.DisposeAsync();

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(syncObj.SceneStack, new IScene[]
                    {
                        new TalkingStickScene(TalkingStickMode.Race),
                    });
                });
        }

        [Fact]
        public async Task TalkingStick_Moderated_PassToParticipant_MakePresenter()
        {
            // arrange
            var (conn, conference) = await ConnectToOpenedConference();
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new TalkingStickScene(TalkingStickMode.Moderated)));

            var pleb = CreateUser();
            await ConnectUserToConference(pleb, conference);

            await conn.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId,
                participants => Assert.Equal(2, participants.Participants.Count));

            // act
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.TalkingStickPass), pleb.Sub));

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(syncObj.SceneStack, new IScene[]
                    {
                        new TalkingStickScene(TalkingStickMode.Moderated), new PresenterScene(pleb.Sub),
                        new ActiveSpeakerScene(),
                    });
                });

            await conn.SyncObjects.AssertSyncObject<SynchronizedSceneTalkingStick>(
                SynchronizedSceneTalkingStick.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => { Assert.Equal(pleb.Sub, syncObj.CurrentSpeakerId); });
        }

        [Fact]
        public async Task TalkingStick_Moderated_Return_RemovePresenter()
        {
            // arrange
            var (conn, conference) = await ConnectToOpenedConference();
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new TalkingStickScene(TalkingStickMode.Moderated)));

            var pleb = CreateUser();
            var plebConn = await ConnectUserToConference(pleb, conference);

            await conn.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId,
                participants => Assert.Equal(2, participants.Participants.Count));

            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.TalkingStickPass), pleb.Sub));
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(syncObj.SceneStack, new IScene[]
                    {
                        new TalkingStickScene(TalkingStickMode.Moderated), new PresenterScene(pleb.Sub),
                        new ActiveSpeakerScene(),
                    });
                });

            // act
            AssertSuccess(await plebConn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.TalkingStickReturn)));

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(syncObj.SceneStack, new IScene[]
                    {
                        new TalkingStickScene(TalkingStickMode.Moderated),
                    });
                });
            await conn.SyncObjects.AssertSyncObject<SynchronizedSceneTalkingStick>(
                SynchronizedSceneTalkingStick.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => { Assert.Null(syncObj.CurrentSpeakerId); });
        }

        [Fact]
        public async Task TalkingStick_Queue_Enqueue_MakePresenter()
        {
            // arrange
            var (conn, conference) = await ConnectToOpenedConference();
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new TalkingStickScene(TalkingStickMode.Queue)));

            var pleb = CreateUser();
            var plebConn = await ConnectUserToConference(pleb, conference);

            // act
            AssertSuccess(await plebConn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.TalkingStickEnqueue)));

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(syncObj.SceneStack, new IScene[]
                    {
                        new TalkingStickScene(TalkingStickMode.Queue), new PresenterScene(pleb.Sub),
                        new ActiveSpeakerScene(),
                    });
                });

            await conn.SyncObjects.AssertSyncObject<SynchronizedSceneTalkingStick>(
                SynchronizedSceneTalkingStick.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Equal(pleb.Sub, syncObj.CurrentSpeakerId);
                    Assert.Empty(syncObj.SpeakerQueue);
                });
        }

        [Fact]
        public async Task TalkingStick_Moderated_Dequeue_RemoveFromQueue()
        {
            // arrange
            var (conn, conference) = await ConnectToOpenedConference();
            AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new TalkingStickScene(TalkingStickMode.Moderated)));

            var pleb = CreateUser();
            var plebConn = await ConnectUserToConference(pleb, conference);

            AssertSuccess(await plebConn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.TalkingStickEnqueue)));
            await conn.SyncObjects.AssertSyncObject<SynchronizedSceneTalkingStick>(
                SynchronizedSceneTalkingStick.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj =>
                {
                    Assert.Null(syncObj.CurrentSpeakerId);
                    Assert.Single(syncObj.SpeakerQueue, pleb.Sub);
                });

            // act
            AssertSuccess(await plebConn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.TalkingStickDequeue)));

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedSceneTalkingStick>(
                SynchronizedSceneTalkingStick.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => { Assert.Empty(syncObj.SpeakerQueue); });
        }
    }
}
