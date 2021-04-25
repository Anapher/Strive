using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Strive.Core.Interfaces;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Hubs.Core;
using Strive.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class SceneTests : ServiceIntegrationTest
    {
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
            Assert.Equal(SceneError.RoomNotFound.Code, result.Error?.Code);
        }

        //[Fact]
        //public async Task JoinToRoom_DoNothing_SynchronizeAvailableScenes()
        //{
        //    // arrange
        //    var (conn, _) = await ConnectToOpenedConference();

        //    // assert
        //    await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
        //        SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
        //        syncObj => { Assert.NotEmpty(syncObj.AvailableScenes); });
        //}

        //[Fact]
        //public async Task SetScene_SceneIsAvailable_UpdateSynchronizedObject()
        //{
        //    // arrange
        //    var (conn, _) = await ConnectToOpenedConference();

        //    // act
        //    var newScene = new ActiveScene(true, ActiveSpeakerScene.Instance, SceneConfig.Default);

        //    var result = await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
        //        new SetSceneDto(RoomOptions.DEFAULT_ROOM_ID, newScene));

        //    // assert
        //    AssertSuccess(result);

        //    await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
        //        SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
        //        syncObj => { Assert.Equal(newScene, syncObj.Active); });
        //}

        //[Fact]
        //public async Task SetScene_SceneIsNotAvailable_ReturnErrorAndDontChangeCurrentScene()
        //{
        //    // arrange
        //    var (conn, _) = await ConnectToOpenedConference();

        //    var current =
        //        await conn.SyncObjects.WaitForSyncObj<SynchronizedScene>(
        //            SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID));

        //    // act
        //    var newScene = new ActiveScene(true, new ScreenShareScene("5342"), SceneConfig.Default);

        //    var result = await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
        //        new SetSceneDto(RoomOptions.DEFAULT_ROOM_ID, newScene));

        //    // assert
        //    AssertFailed(result);

        //    await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
        //        SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
        //        syncObj => { Assert.Equal(current.Active, syncObj.Active); });
        //}

        //[Fact]
        //public async Task OpenBreakoutRooms_Joined_BreakoutRoomSceneBecomesAvailable()
        //{
        //    // arrange
        //    var (conn, _) = await ConnectToOpenedConference();

        //    await conn.SyncObjects.AssertSyncObject(SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
        //        (SynchronizedScene syncObj) =>
        //            Assert.DoesNotContain(syncObj.AvailableScenes, x => x is BreakoutRoomScene));

        //    // act
        //    AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
        //        new OpenBreakoutRoomsDto(5, null, null, null)));

        //    // assert
        //    await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
        //        SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
        //        syncObj => Assert.Contains(syncObj.AvailableScenes, x => x is BreakoutRoomScene));
        //}

        //[Fact]
        //public async Task CloseBreakoutRooms_Joined_BreakoutRoomSceneBecomesUnavailable()
        //{
        //    // arrange
        //    var (conn, _) = await ConnectToOpenedConference();

        //    // act
        //    AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
        //        new OpenBreakoutRoomsDto(5, null, null, null)));

        //    await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
        //        SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
        //        syncObj => Assert.Contains(syncObj.AvailableScenes, x => x is BreakoutRoomScene));

        //    AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseBreakoutRooms)));

        //    // assert
        //    await conn.SyncObjects.AssertSyncObject(SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
        //        (SynchronizedScene syncObj) =>
        //            Assert.DoesNotContain(syncObj.AvailableScenes, x => x is BreakoutRoomScene));
        //}

        //[Fact]
        //public async Task CloseBreakoutRooms_BreakoutRoomIsCurrentScene_RemoveCurrentScene()
        //{
        //    // arrange
        //    var (conn, _) = await ConnectToOpenedConference();

        //    // act
        //    AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
        //        new OpenBreakoutRoomsDto(5, null, null, null)));

        //    AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
        //        new SetSceneDto(RoomOptions.DEFAULT_ROOM_ID,
        //            new ActiveScene(true, new BreakoutRoomScene(), SceneConfig.Default))));

        //    await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
        //        SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
        //        syncObj => Assert.IsType<BreakoutRoomScene>(syncObj.Active.Scene));

        //    AssertSuccess(await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseBreakoutRooms)));

        //    // assert
        //    await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
        //        SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID), syncObj => Assert.Null(syncObj.Active.Scene));
        //}
    }
}
