using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services.Media;
using Strive.Core.Services.Media.Dtos;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Tests._TestHelpers;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.Providers
{
    public class ScreenShareSceneProviderTests
    {
        private readonly Mock<IMediator> _mediator = new();

        private const string ConferenceId = "123";
        private const string RoomId = "room1";

        private readonly ParticipantStreams _activeScreenProducer =
            new(ImmutableDictionary<string, ConsumerInfo>.Empty, new Dictionary<ProducerSource, ProducerInfo>
            {
                {ProducerSource.Screen, new ProducerInfo(false)},
            });

        private SynchronizedRooms CreateRooms(IReadOnlyDictionary<string, string> participants)
        {
            return new(ImmutableList<Room>.Empty, "default", participants);
        }

        [Fact]
        public async Task GetAvailableScenes_ParticipantsInRoomAreScreenSharing_ReturnScreenShareScenes()
        {
            // arrange
            _mediator.SetupSyncObj(SynchronizedRooms.SyncObjId, CreateRooms(new Dictionary<string, string>
            {
                {"p1", RoomId}, {"p2", RoomId},
                {"p3", "room2"},
            }));

            _mediator.SetupSyncObj(SynchronizedMediaState.SyncObjId, new SynchronizedMediaState(
                new Dictionary<string, ParticipantStreams>
                {
                    {
                        "p1",
                        _activeScreenProducer
                    },
                    {"p2", _activeScreenProducer},
                }));

            var provider = new ScreenShareSceneProvider(_mediator.Object);

            // act
            var scenes = await provider.GetAvailableScenes(ConferenceId, RoomId, ImmutableList<IScene>.Empty);

            // assert
            Assert.Single(scenes, new ScreenShareScene("p1"));
        }

        [Fact]
        public async Task IsUpdateRequired_RoomNotChanged_ReturnFalse()
        {
            // arrange
            var provider = new ScreenShareSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId, string.Empty,
                CreateRooms(new Dictionary<string, string> {{"p1", RoomId}}),
                CreateRooms(new Dictionary<string, string> {{"p1", RoomId}}));

            // assert
            Assert.Equal(SceneUpdate.NotRequired, result);
        }

        [Fact]
        public async Task IsUpdateRequired_RoomChanged_ReturnTrue()
        {
            // arrange
            var provider = new ScreenShareSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId, string.Empty,
                CreateRooms(new Dictionary<string, string> {{"p1", RoomId}}),
                CreateRooms(new Dictionary<string, string> {{"p1", "other room"}}));

            // assert
            Assert.Equal(SceneUpdate.AvailableScenesChanged, result);
        }

        [Fact]
        public async Task IsUpdateRequired_SyncMediaChanged_ReturnTrue()
        {
            // arrange
            var provider = new ScreenShareSceneProvider(_mediator.Object);

            // act
            var result = await provider.IsUpdateRequired(ConferenceId, RoomId, string.Empty,
                new SynchronizedMediaState(ImmutableDictionary<string, ParticipantStreams>.Empty), null);

            // assert
            Assert.Equal(SceneUpdate.AvailableScenesChanged, result);
        }
    }
}
