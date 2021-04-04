using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Scenes;
using PaderConference.Core.Services.Scenes.Modes;
using PaderConference.Hubs.Core;
using PaderConference.Hubs.Core.Dtos;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Services
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
            var (conn, _) = await ConnectToOpenedConference();

            // act
            var result = await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new SetSceneDto("21345", new ActiveScene(false, AutonomousScene.Instance, SceneConfig.Default)));

            // assert
            AssertFailed(result);
            Assert.Equal(SceneError.RoomNotFound.Code, result.Error?.Code);
        }

        [Fact]
        public async Task JoinToRoom_DoNothing_SynchronizedAvailableScenes()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // assert
            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => { Assert.NotEmpty(syncObj.AvailableScenes); });
        }

        [Fact]
        public async Task SetScene_RoomDoesExist_UpdateSynchronizedObject()
        {
            // arrange
            var (conn, _) = await ConnectToOpenedConference();

            // act
            var newScene = new ActiveScene(true, ActiveSpeakerScene.Instance, SceneConfig.Default);

            var result = await conn.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetScene),
                new SetSceneDto(RoomOptions.DEFAULT_ROOM_ID, newScene));

            // assert
            AssertSuccess(result);

            await conn.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                syncObj => { Assert.Equal(newScene, syncObj.Active); });
        }
    }
}
