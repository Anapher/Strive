using System.Collections.Immutable;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Tests._TestHelpers;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.Providers
{
    public class BreakoutRoomSceneProviderTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "room1";

        private readonly Mock<IMediator> _mediator = new();

        private readonly SynchronizedBreakoutRooms _inactiveBreakoutRooms = new(null);

        private readonly SynchronizedBreakoutRooms _activeBreakoutRooms = new(new BreakoutRoomsConfig(5, null, null));

        [Fact]
        public async Task GetAvailableScenes_IsNotActive_ReturnEmpty()
        {
            // arrange
            _mediator.SetupSyncObj(SynchronizedBreakoutRooms.SyncObjId, _inactiveBreakoutRooms);

            var provider = new BreakoutRoomSceneProvider(_mediator.Object);

            // act
            var result = await provider.GetAvailableScenes(ConferenceId, RoomId, ImmutableList<IScene>.Empty);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableScenes_IsActive_ReturnScene()
        {
            // arrange
            _mediator.SetupSyncObj(SynchronizedBreakoutRooms.SyncObjId, _activeBreakoutRooms);

            var provider = new BreakoutRoomSceneProvider(_mediator.Object);

            // act
            var result = await provider.GetAvailableScenes(ConferenceId, RoomId, ImmutableList<IScene>.Empty);

            // assert
            Assert.Single(result, BreakoutRoomScene.Instance);
        }

        [Fact]
        public async Task IsUpdateRequired_CurrentInactiveAndPreviousNull_ReturnFalse()
        {
            // arrange
            var provider = new BreakoutRoomSceneProvider(_mediator.Object);

            // act
            var result =
                await provider.IsUpdateRequired(ConferenceId, RoomId, string.Empty, _inactiveBreakoutRooms, null);

            // assert
            Assert.Equal(SceneUpdate.NotRequired, result);
        }

        [Fact]
        public async Task IsUpdateRequired_CurrentInactiveAndPreviousInactive_ReturnFalse()
        {
            // arrange
            var provider = new BreakoutRoomSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId, string.Empty, _inactiveBreakoutRooms,
                _inactiveBreakoutRooms);

            // assert
            Assert.Equal(SceneUpdate.NotRequired, result);
        }

        [Fact]
        public async Task IsUpdateRequired_CurrentActiveAndPreviousInactive_ReturnTrue()
        {
            // arrange
            var provider = new BreakoutRoomSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId, string.Empty, _activeBreakoutRooms,
                _inactiveBreakoutRooms);

            // assert
            Assert.Equal(SceneUpdate.AvailableScenesChanged, result);
        }

        [Fact]
        public async Task IsUpdateRequired_CurrentInactiveAndPreviousActive_ReturnTrue()
        {
            // arrange
            var provider = new BreakoutRoomSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId, string.Empty, _inactiveBreakoutRooms,
                _activeBreakoutRooms);

            // assert
            Assert.Equal(SceneUpdate.AvailableScenesChanged, result);
        }

        [Fact]
        public async Task IsUpdateRequired_CurrentActiveAndPreviousActive_ReturnFalse()
        {
            // arrange
            var provider = new BreakoutRoomSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId, string.Empty, _activeBreakoutRooms,
                _activeBreakoutRooms);

            // assert
            Assert.Equal(SceneUpdate.NotRequired, result);
        }
    }
}
