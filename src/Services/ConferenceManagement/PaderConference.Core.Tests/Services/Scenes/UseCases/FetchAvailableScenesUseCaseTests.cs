using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Scenes;
using PaderConference.Core.Services.Scenes.Gateways;
using PaderConference.Core.Services.Scenes.Modes;
using PaderConference.Core.Services.Scenes.Requests;
using PaderConference.Core.Services.Scenes.UseCases;
using Xunit;

namespace PaderConference.Core.Tests.Services.Scenes.UseCases
{
    public class FetchAvailableScenesUseCaseTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "45";

        private readonly Mock<ISceneProvider> _sceneProvider = new();
        private readonly Mock<ISceneRepository> _repository = new();

        private FetchAvailableScenesUseCase Create()
        {
            return new(_repository.Object, _sceneProvider.Object.Yield());
        }

        [Fact]
        public async Task Handle_AvailableScenesCached_DontCallProvidersAndReturnCachedResult()
        {
            // arrange
            _repository.Setup(x => x.GetAvailableScenes(ConferenceId, RoomId))
                .ReturnsAsync(new List<IScene> {AutonomousScene.Instance});
            var useCase = Create();

            // act
            var result = await useCase.Handle(new FetchAvailableScenesRequest(ConferenceId, RoomId),
                CancellationToken.None);

            // assert
            Assert.Single(result, AutonomousScene.Instance);
            _sceneProvider.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_NotCached_FetchResultFromProvidersAndCache()
        {
            // arrange
            _sceneProvider.Setup(x => x.GetAvailableScenes(ConferenceId, RoomId))
                .ReturnsAsync(new List<IScene> {AutonomousScene.Instance});

            var useCase = Create();

            // act
            var result = await useCase.Handle(new FetchAvailableScenesRequest(ConferenceId, RoomId),
                CancellationToken.None);

            // assert
            Assert.Single(result, AutonomousScene.Instance);
            _repository.Verify(
                x => x.SetAvailableScenes(ConferenceId, RoomId,
                    It.Is<IReadOnlyList<IScene>>(param => ReferenceEquals(param.Single(), AutonomousScene.Instance))),
                Times.Once);
        }
    }
}
