using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Strive.Core.Extensions;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Scenes.UseCases;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.UseCases
{
    public class FetchAvailableScenesUseCaseTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "45";

        private readonly Mock<ISceneProvider> _sceneProvider = new();

        private FetchAvailableScenesUseCase Create()
        {
            return new(_sceneProvider.Object.Yield());
        }

        [Fact]
        public async Task Handle_OneProvider_FetchResultFromProvidersAndCache()
        {
            // arrange
            var useCase = Create();
            var sceneStack = new List<IScene> {new Mock<IScene>().Object};

            _sceneProvider.Setup(x => x.GetAvailableScenes(ConferenceId, RoomId, sceneStack)).ReturnsAsync(sceneStack);

            // act
            var result = await useCase.Handle(new FetchAvailableScenesRequest(ConferenceId, RoomId, sceneStack),
                CancellationToken.None);

            // assert
            Assert.Single(result, sceneStack.Single());
        }
    }
}
