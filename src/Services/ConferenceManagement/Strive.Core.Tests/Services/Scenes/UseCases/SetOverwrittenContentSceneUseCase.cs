using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Scenes.UseCases;
using Strive.Core.Services.Synchronization.Requests;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.UseCases
{
    public class SetOverwrittenContentSceneUseCaseTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "45";

        private readonly Mock<ISceneRepository> _sceneRepository = new();
        private readonly Mock<IMediator> _mediator = new();

        private SetOverwrittenContentSceneUseCase Create()
        {
            return new(_mediator.Object, _sceneRepository.Object);
        }

        private void SetupRooms(params Room[] rooms)
        {
            var syncObj = new SynchronizedRooms(rooms, "default", ImmutableDictionary<string, string>.Empty);

            _mediator.Setup(x => x.Send(It.Is<FetchSynchronizedObjectRequest>(req => req.ConferenceId == ConferenceId),
                It.IsAny<CancellationToken>())).ReturnsAsync(syncObj);
        }

        [Fact]
        public async Task Handle_PatchScene_SetInRepository()
        {
            // arrange
            SetupRooms(new Room(RoomId, "DefaultRoom"));

            var useCase = Create();

            // act
            var newScene = GridScene.Instance;
            await useCase.Handle(new SetOverwrittenContentSceneRequest(ConferenceId, RoomId, newScene),
                CancellationToken.None);

            // assert
            _sceneRepository.Verify(
                x => x.SetScene(ConferenceId, RoomId,
                    It.Is<ActiveScene>(scene => Equals(scene.OverwrittenContent, newScene))), Times.Once);
        }

        [Fact]
        public async Task Handle_PatchScene_SendUpdateScenes()
        {
            // arrange
            SetupRooms(new Room(RoomId, "DefaultRoom"));

            var useCase = Create();

            // act
            var newScene = GridScene.Instance;
            await useCase.Handle(new SetOverwrittenContentSceneRequest(ConferenceId, RoomId, newScene),
                CancellationToken.None);

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateScenesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
