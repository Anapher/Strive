using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Scenes;
using PaderConference.Core.Services.Scenes.Dto;
using PaderConference.Core.Services.Scenes.Modes;
using PaderConference.Core.Signaling;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Scenes
{
    public class ScenesServiceTests : ServiceTest<ScenesService>
    {
        private MockPermissionsService _permissionsService = new MockPermissionsService(
            new Dictionary<string, IReadOnlyDictionary<string, JsonElement>>
            {
                {TestParticipants.Default.ParticipantId, new Dictionary<string, JsonElement>()},
            });

        private readonly ScenesOptions _scenesOptions = new ScenesOptions();
        private readonly MockSynchronizationManager _synchronizationManager = new MockSynchronizationManager();
        private readonly Mock<ISignalMessenger> _signalMessenger = new Mock<ISignalMessenger>();

        private readonly Mock<IRoomManagement> _roomManagementMock = new Mock<IRoomManagement>();

        public ScenesServiceTests(ITestOutputHelper output) : base(output)
        {
            _scenesOptions = new ScenesOptions
            {
                DefaultRoomState = new RoomSceneState {IsControlled = true, Scene = AutomaticScene.Instance},
                RoomState = new RoomSceneState {IsControlled = true, Scene = AutomaticScene.Instance},
            };
        }

        private ScenesService Create()
        {
            return new ScenesService(_roomManagementMock.Object, _synchronizationManager,
                new OptionsWrapper<ScenesOptions>(_scenesOptions), _permissionsService, Logger);
        }

        private IImmutableDictionary<string, RoomSceneState> GetCurrentState()
        {
            return (IImmutableDictionary<string, RoomSceneState>) _synchronizationManager.Objects["scenes"]
                .GetCurrent();
        }

        private void EnableSetScenePermissions()
        {
            _permissionsService = new MockPermissionsService(
                new Dictionary<string, IReadOnlyDictionary<string, JsonElement>>
                {
                    {
                        TestParticipants.Default.ParticipantId,
                        new[] {PermissionsList.Scenes.CanSetScene.Configure(true)}.ToDictionary(x => x.Key,
                            x => x.Value)
                    },
                });
        }

        [Fact]
        public async Task TestInitializeExistingRoomsNoneExist()
        {
            // arrange
            _roomManagementMock.SetupGet(x => x.State).Returns(new ConferenceRooms(ImmutableList<Room>.Empty, "456",
                ImmutableDictionary<string, string>.Empty));

            var service = Create();

            // act
            await service.InitializeAsync();

            // assert
            var result = GetCurrentState();
            Assert.Empty(result);
        }

        [Fact]
        public async Task TestInitializeExistingRooms()
        {
            // arrange
            _roomManagementMock.SetupGet(x => x.State).Returns(new ConferenceRooms(
                new[] {new Room("456", "master", true)}.ToImmutableList(), "456",
                ImmutableDictionary<string, string>.Empty));

            var service = Create();

            // act
            await service.InitializeAsync();

            // assert
            var result = GetCurrentState();
            Assert.Single(result);
        }

        [Fact]
        public async Task TestAddDefaultRoom()
        {
            // arrange
            _scenesOptions.RoomState = new RoomSceneState {IsControlled = true, Scene = AutomaticScene.Instance};
            _scenesOptions.DefaultRoomState = new RoomSceneState {IsControlled = false, Scene = new GridScene()};

            var service = Create();
            _roomManagementMock.SetupGet(x => x.State).Returns(new ConferenceRooms(ImmutableList<Room>.Empty, "123",
                ImmutableDictionary<string, string>.Empty));

            // act

            await service.InitializeAsync();
            _roomManagementMock.Raise(x => x.RoomsCreated += null, _roomManagementMock.Object,
                new List<Room> {new Room("123", "master", true)});

            // assert
            var result = GetCurrentState();
            var scene = Assert.Single(result);

            Assert.Equal("123", scene.Key);
            Assert.Equal(_scenesOptions.DefaultRoomState.IsControlled, scene.Value.IsControlled);
            Assert.Equal(_scenesOptions.DefaultRoomState.Scene?.Type, scene.Value.Scene?.Type);
        }

        [Fact]
        public async Task TestAddRoom()
        {
            // arrange
            _scenesOptions.RoomState = new RoomSceneState {IsControlled = true, Scene = AutomaticScene.Instance};
            _scenesOptions.DefaultRoomState = new RoomSceneState {IsControlled = false, Scene = new GridScene()};

            var service = Create();
            _roomManagementMock.SetupGet(x => x.State).Returns(new ConferenceRooms(ImmutableList<Room>.Empty, "123",
                ImmutableDictionary<string, string>.Empty));

            // act

            await service.InitializeAsync();
            _roomManagementMock.Raise(x => x.RoomsCreated += null, _roomManagementMock.Object,
                new List<Room> {new Room("456", "master", true)});

            // assert
            var result = GetCurrentState();
            var scene = Assert.Single(result);

            Assert.Equal("456", scene.Key);
            Assert.Equal(_scenesOptions.RoomState.IsControlled, scene.Value.IsControlled);
            Assert.Equal(_scenesOptions.RoomState.Scene?.Type, scene.Value.Scene?.Type);
        }

        [Fact]
        public async Task TestRemoveRoom()
        {
            // arrange
            var service = Create();
            _roomManagementMock.SetupGet(x => x.State).Returns(new ConferenceRooms(
                new[] {new Room("456", "master", true)}.ToImmutableList(), "456",
                ImmutableDictionary<string, string>.Empty));

            // act
            await service.InitializeAsync();
            _roomManagementMock.Raise(x => x.RoomsRemoved += null, _roomManagementMock.Object,
                new List<string> {"456"});

            // assert
            var result = GetCurrentState();
            Assert.Empty(result);
        }

        [Fact]
        public async Task TestSetRoomSceneNoPermissions()
        {
            // arrange
            var service = Create();
            _roomManagementMock.SetupGet(x => x.State).Returns(new ConferenceRooms(
                new[] {new Room("456", "master", true)}.ToImmutableList(), "456",
                ImmutableDictionary<string, string>.Empty));

            var messageMock = TestServiceMessage.Create(
                new ChangeSceneDto
                {
                    RoomId = "456", Scene = new RoomSceneState {Scene = new AutomaticScene(), IsControlled = false},
                }, TestParticipants.Default, "123");

            // act
            await service.InitializeAsync();
            await service.SetScene(messageMock.Object);

            // assert
            var result = GetCurrentState();
            var scene = Assert.Single(result);
            Assert.True(scene.Value.IsControlled);

            messageMock.Verify(x => x.SendToCallerAsync(CoreHubMessages.Response.OnError, It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task TestSetRoomSceneRoomNotExisting()
        {
            // arrange
            EnableSetScenePermissions();

            var service = Create();
            _roomManagementMock.SetupGet(x => x.State).Returns(new ConferenceRooms(ImmutableList<Room>.Empty, "456",
                ImmutableDictionary<string, string>.Empty));

            var messageMock = TestServiceMessage.Create(new ChangeSceneDto {RoomId = "123", Scene = null},
                TestParticipants.Default, "123");

            // act
            await service.InitializeAsync();
            await service.SetScene(messageMock.Object);

            // assert
            var result = GetCurrentState();
            Assert.Empty(result);

            messageMock.Verify(x => x.SendToCallerAsync(CoreHubMessages.Response.OnError, It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task TestSetRoomSceneRoom()
        {
            // arrange
            EnableSetScenePermissions();

            var service = Create();
            _roomManagementMock.SetupGet(x => x.State).Returns(new ConferenceRooms(
                new[] {new Room("456", "master", true)}.ToImmutableList(), "456",
                ImmutableDictionary<string, string>.Empty));

            var messageMock = TestServiceMessage.Create(
                new ChangeSceneDto
                {
                    RoomId = "456", Scene = new RoomSceneState {Scene = new AutomaticScene(), IsControlled = false},
                }, TestParticipants.Default, "123");

            // act
            await service.InitializeAsync();
            await service.SetScene(messageMock.Object);

            // assert
            var result = GetCurrentState();
            var scene = Assert.Single(result);
            Assert.False(scene.Value.IsControlled);

            messageMock.Verify(x => x.SendToCallerAsync(CoreHubMessages.Response.OnError, It.IsAny<object>()),
                Times.Never);
        }
    }
}
