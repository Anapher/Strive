using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Extensions;
using Strive.Core.Services;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Modes;
using Strive.Core.Services.Scenes.NotificationHandlers;
using Strive.Core.Services.Synchronization.Notifications;
using Strive.Core.Services.Synchronization.Requests;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.NotificationHandlers
{
    public class SynchronizedObjectUpdatedNotificationHandlerTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<ISceneProvider> _sceneProvider = new();
        private readonly Mock<ISceneRepository> _repository = new();

        private const string ConferenceId = "123";
        private const string RoomId = "45";

        private SynchronizedObjectUpdatedNotificationHandler Create()
        {
            return new(_mediator.Object, _sceneProvider.Object.Yield(), _repository.Object);
        }

        private void SetupRooms(params Room[] rooms)
        {
            var syncObj = new SynchronizedRooms(rooms, "default", ImmutableDictionary<string, string>.Empty);

            _mediator.Setup(x => x.Send(It.Is<FetchSynchronizedObjectRequest>(req => req.ConferenceId == ConferenceId),
                It.IsAny<CancellationToken>())).ReturnsAsync(syncObj);
        }

        [Fact]
        public async Task Handle_PreviousValueNull_DoNothing()
        {
            // arrange
            var useCase = Create();

            // act
            await useCase.Handle(
                new SynchronizedObjectUpdatedNotification(ImmutableList<Participant>.Empty, "test", "hey", null),
                CancellationToken.None);

            // assert
            _repository.VerifyNoOtherCalls();
            _sceneProvider.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_NoUpdatesAvailable_DoNothing()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, ""));

            _sceneProvider.Setup(x => x.UpdateAvailableScenes(ConferenceId, RoomId, It.IsAny<object>()))
                .ReturnsAsync(SceneUpdate.NoUpdateRequired);

            // act
            var participants = new List<Participant> {new(ConferenceId, "1")};
            await useCase.Handle(new SynchronizedObjectUpdatedNotification(participants, "123", "hello", "hllo"),
                CancellationToken.None);

            // assert
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_UpdatesAvailableButCachedAvailableScenesNull_DoNothing()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, ""));

            _sceneProvider.Setup(x => x.UpdateAvailableScenes(ConferenceId, RoomId, It.IsAny<object>()))
                .ReturnsAsync(SceneUpdate.UpdateRequired(new[] {new ScreenShareScene("1")}));

            // act
            var participants = new List<Participant> {new(ConferenceId, "1")};
            await useCase.Handle(new SynchronizedObjectUpdatedNotification(participants, "123", "hello", "hllo"),
                CancellationToken.None);

            // assert
            _repository.Verify(x => x.GetAvailableScenes(ConferenceId, RoomId));
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_NewUpdate_UpdateAvailableScenesAndUpdateSyncObj()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, ""));

            _sceneProvider.Setup(x => x.UpdateAvailableScenes(ConferenceId, RoomId, It.IsAny<object>()))
                .ReturnsAsync(SceneUpdate.UpdateRequired(new[] {new ScreenShareScene("1")}));
            _sceneProvider.Setup(x => x.IsProvided(It.IsAny<ScreenShareScene>())).Returns(true);

            _repository.Setup(x => x.GetAvailableScenes(ConferenceId, RoomId))
                .ReturnsAsync(new IScene[] {AutonomousScene.Instance, new ScreenShareScene("2")});

            // act
            var participants = new List<Participant> {new(ConferenceId, "1")};
            await useCase.Handle(new SynchronizedObjectUpdatedNotification(participants, "123", "hello", "hllo"),
                CancellationToken.None);

            // assert
            _repository.Verify(
                x => x.SetAvailableScenes(ConferenceId, RoomId,
                    It.Is<IReadOnlyList<IScene>>(val =>
                        val.Count == 2 && val.Contains(AutonomousScene.Instance) &&
                        val.Contains(new ScreenShareScene("1")))), Times.Once);

            _mediator.Verify(x => x.Send(It.IsAny<UpdateSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_NewUpdateAndCurrentSceneDoesntExistAnymore_RemoveCurrentScene()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, ""));

            _repository.Setup(x => x.GetScene(ConferenceId, RoomId))
                .ReturnsAsync(new ActiveScene(true, new ScreenShareScene("123"), SceneConfig.Default));

            _sceneProvider.Setup(x => x.UpdateAvailableScenes(ConferenceId, RoomId, It.IsAny<object>()))
                .ReturnsAsync(SceneUpdate.UpdateRequired(new[] {new ScreenShareScene("1")}));
            _sceneProvider.Setup(x => x.IsProvided(It.IsAny<ScreenShareScene>())).Returns(true);

            _repository.Setup(x => x.GetAvailableScenes(ConferenceId, RoomId))
                .ReturnsAsync(new IScene[] {AutonomousScene.Instance, new ScreenShareScene("2")});

            // act
            var participants = new List<Participant> {new(ConferenceId, "1")};
            await useCase.Handle(new SynchronizedObjectUpdatedNotification(participants, "123", "hello", "hllo"),
                CancellationToken.None);

            // assert
            _repository.Verify(x => x.SetScene(ConferenceId, RoomId, It.Is<ActiveScene>(x => x.Scene == null)),
                Times.Once);
        }
    }
}
