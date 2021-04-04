using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Scenes;
using PaderConference.Core.Services.Scenes.Gateways;
using PaderConference.Core.Services.Scenes.Modes;
using PaderConference.Core.Services.Scenes.Requests;
using PaderConference.Core.Services.Scenes.UseCases;
using PaderConference.Core.Services.Synchronization.Requests;
using Xunit;

namespace PaderConference.Core.Tests.Services.Scenes.UseCases
{
    public class SetSceneUseCaseTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "45";

        private readonly Mock<ISceneRepository> _sceneRepository = new();
        private readonly Mock<IMediator> _mediator = new();

        private SetSceneUseCase Create()
        {
            return new(_sceneRepository.Object, _mediator.Object);
        }

        private void SetupRooms(params Room[] rooms)
        {
            var syncObj = new SynchronizedRooms(rooms, "default", ImmutableDictionary<string, string>.Empty);

            _mediator.Setup(x => x.Send(It.Is<FetchSynchronizedObjectRequest>(req => req.ConferenceId == ConferenceId),
                It.IsAny<CancellationToken>())).ReturnsAsync(syncObj);
        }

        [Fact]
        public async Task Handle_SynchronizedRoomsNull_DontUpdateSyncObjAndDontSetInRepo()
        {
            // arrange
            var useCase = Create();

            // act
            await Assert.ThrowsAnyAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new SetSceneRequest(ConferenceId, RoomId, new ActiveScene(false, null, SceneConfig.Default)),
                    CancellationToken.None));

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _sceneRepository.Verify(x => x.RemoveScene(ConferenceId, RoomId), Times.Once);
        }

        [Fact]
        public async Task Handle_RoomDoesNotExist_DontUpdateSyncObjAndDontSetInRepo()
        {
            // arrange
            var useCase = Create();
            SetupRooms();

            // act
            await Assert.ThrowsAnyAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new SetSceneRequest(ConferenceId, RoomId, new ActiveScene(false, null, SceneConfig.Default)),
                    CancellationToken.None));

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _sceneRepository.Verify(x => x.RemoveScene(ConferenceId, RoomId), Times.Once);
        }

        [Fact]
        public async Task Handle_RoomExists_UpdateSyncObj()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, "DefaultRoom"));

            // act
            await useCase.Handle(
                new SetSceneRequest(ConferenceId, RoomId, new ActiveScene(false, null, SceneConfig.Default)),
                CancellationToken.None);

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _sceneRepository.Verify(x => x.SetScene(ConferenceId, RoomId, It.IsAny<ActiveScene>()), Times.Once);
            _sceneRepository.Verify(x => x.RemoveScene(ConferenceId, RoomId), Times.Never);
        }

        [Fact]
        public async Task Handle_NoActiveSceneAndNewSceneNotAvailable_ThrowAndSetNull()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, "DefaultRoom"));

            _mediator.Setup(x => x.Send(It.IsAny<FetchAvailableScenesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<IScene>());

            // act
            await Assert.ThrowsAnyAsync<IdErrorException>(async () =>
            {
                await useCase.Handle(
                    new SetSceneRequest(ConferenceId, RoomId,
                        new ActiveScene(false, AutonomousScene.Instance, SceneConfig.Default)),
                    CancellationToken.None);
            });

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _sceneRepository.Verify(x => x.RemoveScene(ConferenceId, RoomId), Times.Once);
        }

        [Fact]
        public async Task Handle_HasActiveSceneAndNewSceneNotAvailable_ThrowAndSetToPreviousScene()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, "DefaultRoom"));

            var previousScene = new ActiveScene(false, ActiveSpeakerScene.Instance, SceneConfig.Default);

            _sceneRepository.Setup(x => x.GetScene(ConferenceId, RoomId)).ReturnsAsync(previousScene);

            _mediator.Setup(x => x.Send(It.IsAny<FetchAvailableScenesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<IScene>());

            // act
            await Assert.ThrowsAnyAsync<IdErrorException>(async () =>
            {
                await useCase.Handle(
                    new SetSceneRequest(ConferenceId, RoomId,
                        new ActiveScene(false, AutonomousScene.Instance, SceneConfig.Default)),
                    CancellationToken.None);
            });

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _sceneRepository.Verify(x => x.SetScene(ConferenceId, RoomId, previousScene), Times.Once);
        }

        [Fact]
        public async Task Handle_NewSceneIsAvailable_SetNewScene()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, "DefaultRoom"));

            _mediator.Setup(x => x.Send(It.IsAny<FetchAvailableScenesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<IScene> {new ScreenShareScene("213")});

            // act
            await useCase.Handle(
                new SetSceneRequest(ConferenceId, RoomId,
                    new ActiveScene(false, new ScreenShareScene("213"), SceneConfig.Default)), CancellationToken.None);

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
