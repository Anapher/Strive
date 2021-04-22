using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers;
using Strive.Core.Services.Scenes.Scenes;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.Providers
{
    public class AutonomousSceneProviderTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "room1";

        private readonly SceneStackFunc _emptyStackFunc =
            (_, _) => new ValueTask<IEnumerable<IScene>>(ImmutableList<IScene>.Empty);

        [Fact]
        public async Task GetAvailableScenes_IsAlwaysAvailable()
        {
            // arrange
            var provider = new AutonomousSceneProvider();

            // act
            var availableScenes = await provider.GetAvailableScenes(ConferenceId, RoomId, ImmutableList<IScene>.Empty);

            // assert
            Assert.Single(availableScenes, AutonomousScene.Instance);
        }

        [Fact]
        public async Task BuildStack_NoScreenShareScenes_ReturnDefault()
        {
            // arrange
            var provider = new AutonomousSceneProvider();
            var context = new SceneBuilderContext(ConferenceId, RoomId, ImmutableList<IScene>.Empty,
                new SceneOptions {DefaultScene = SceneOptions.BasicSceneType.Grid}, ImmutableList<IScene>.Empty);

            // act
            var availableScenes = await provider.BuildStack(AutonomousScene.Instance, context, _emptyStackFunc);

            // assert
            Assert.Equal(availableScenes, new IScene[] {AutonomousScene.Instance, GridScene.Instance});
        }

        [Fact]
        public async Task BuildStack_NewScreenShare_ReturnScreen()
        {
            // arrange
            var provider = new AutonomousSceneProvider();
            var context = new SceneBuilderContext(ConferenceId, RoomId, new[] {new ScreenShareScene("p1")},
                new SceneOptions(), ImmutableList<IScene>.Empty);

            // act
            var availableScenes = await provider.BuildStack(AutonomousScene.Instance, context, _emptyStackFunc);

            // assert
            Assert.Equal(availableScenes, new IScene[] {AutonomousScene.Instance, new ScreenShareScene("p1")});
        }

        [Fact]
        public async Task BuildStack_NewScreenShareButAlreadyActiveScreenShare_ReturnPreviousScreenShare()
        {
            // arrange
            var provider = new AutonomousSceneProvider();
            var context = new SceneBuilderContext(ConferenceId, RoomId,
                new[] {new ScreenShareScene("p2"), new ScreenShareScene("p1")}, new SceneOptions(),
                new[] {new ScreenShareScene("p1")});

            // act
            var availableScenes = await provider.BuildStack(AutonomousScene.Instance, context, _emptyStackFunc);

            // assert
            Assert.Equal(availableScenes, new IScene[] {AutonomousScene.Instance, new ScreenShareScene("p1")});
        }
    }
}
