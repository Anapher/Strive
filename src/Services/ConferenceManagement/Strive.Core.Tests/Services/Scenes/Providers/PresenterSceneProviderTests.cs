using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Tests._TestHelpers;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.Providers
{
    public class PresenterSceneProviderTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "room1";

        private readonly Mock<IMediator> _mediator = new();

        private readonly SceneStackFunc _emptyStackFunc =
            (_, _) => new ValueTask<IEnumerable<IScene>>(ImmutableList<IScene>.Empty);

        private SynchronizedRooms CreateRooms(IReadOnlyDictionary<string, string> participants)
        {
            return new(ImmutableList<Room>.Empty, "default", participants);
        }

        [Fact]
        public async Task GetAvailableScenes_NoPresenterScenesInStack_ReturnEmpty()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            // act
            var result = await provider.GetAvailableScenes(ConferenceId, RoomId, ImmutableList<IScene>.Empty);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableScenes_PresenterScenesInStackAndParticipantJoined_ReturnScene()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            _mediator.SetupSyncObj(SynchronizedRooms.SyncObjId,
                CreateRooms(new Dictionary<string, string> {{"p1", RoomId}}));

            // act
            var result = await provider.GetAvailableScenes(ConferenceId, RoomId, new[] {new PresenterScene("p1")});

            // assert
            Assert.Single(result, new PresenterScene("p1"));
        }

        [Fact]
        public async Task GetAvailableScenes_PresenterScenesInStackAndParticipantNotJoined_ReturnScene()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            _mediator.SetupSyncObj(SynchronizedRooms.SyncObjId,
                CreateRooms(new Dictionary<string, string> {{"p1", "no room"}}));

            // act
            var result = await provider.GetAvailableScenes(ConferenceId, RoomId, new[] {new PresenterScene("p1")});

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task IsUpdateRequired_RoomNotChanged_ReturnFalse()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId,
                CreateRooms(new Dictionary<string, string> {{"p1", RoomId}}),
                CreateRooms(new Dictionary<string, string> {{"p1", RoomId}}));

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsUpdateRequired_RoomChanged_ReturnTrue()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId,
                CreateRooms(new Dictionary<string, string> {{"p1", RoomId}}),
                CreateRooms(new Dictionary<string, string> {{"p1", "other room"}}));

            // assert
            Assert.True(result);
        }

        [Fact]
        public async Task BuildStack_NoScreenShare_ReturnActiveSpeaker()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            // act
            var result = await provider.BuildStack(new PresenterScene("p1"),
                new SceneBuilderContext(ConferenceId, RoomId, ImmutableList<IScene>.Empty, new SceneOptions(),
                    ImmutableList<IScene>.Empty), _emptyStackFunc);

            // assert
            Assert.Equal(new IScene[] {new PresenterScene("p1"), ActiveSpeakerScene.Instance}, result);
        }

        [Fact]
        public async Task BuildStack_ScreenShare_ReturnScreenShare()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            // act
            var result = await provider.BuildStack(new PresenterScene("p1"),
                new SceneBuilderContext(ConferenceId, RoomId, new IScene[] {new ScreenShareScene("p1")},
                    new SceneOptions(), ImmutableList<IScene>.Empty), _emptyStackFunc);

            // assert
            Assert.Equal(new IScene[] {new PresenterScene("p1"), new ScreenShareScene("p1")}, result);
        }

        [Fact]
        public async Task FetchPermissionsForParticipant_NotPresenter_ReturnEmpty()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            // act
            var layers = await provider.FetchPermissionsForParticipant(new PresenterScene("p1"),
                new Participant(ConferenceId, "p5"), ImmutableList<IScene>.Empty);

            // assert
            Assert.Empty(layers);
        }

        [Fact]
        public async Task FetchPermissionsForParticipant_IsPresenter_ReturnLayer()
        {
            // arrange
            var provider = new PresenterSceneProvider(_mediator.Object);

            // act
            var layers = await provider.FetchPermissionsForParticipant(new PresenterScene("p1"),
                new Participant(ConferenceId, "p1"), ImmutableList<IScene>.Empty);

            // assert
            Assert.Single(layers);
        }
    }
}
