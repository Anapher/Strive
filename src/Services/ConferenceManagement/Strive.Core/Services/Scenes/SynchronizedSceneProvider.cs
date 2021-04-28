using System.Collections.Immutable;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Scenes
{
    public class SynchronizedSceneProvider : SynchronizedObjectProviderForRoom<SynchronizedScene>
    {
        private readonly ISceneRepository _sceneRepository;

        public SynchronizedSceneProvider(ISceneRepository sceneRepository, IMediator mediator) : base(mediator)
        {
            _sceneRepository = sceneRepository;
        }

        public override string Id => SynchronizedObjectIds.SCENE;

        protected override async ValueTask<SynchronizedScene> InternalFetchValue(string conferenceId, string roomId)
        {
            var scene = await _sceneRepository.GetScene(conferenceId, roomId);
            scene ??= GetDefaultActiveScene();

            var state = await _sceneRepository.GetSceneState(conferenceId, roomId);
            state ??= GetEmptySceneState();

            return new SynchronizedScene(scene.SelectedScene, scene.OverwrittenContent, state.AvailableScenes,
                state.SceneStack);
        }

        public static ActiveScene GetDefaultActiveScene()
        {
            return new(GetDefaultScene(), null);
        }

        public static IScene GetDefaultScene()
        {
            return AutonomousScene.Instance;
        }

        public static SceneState GetEmptySceneState()
        {
            return new(ImmutableList<IScene>.Empty, ImmutableList<IScene>.Empty);
        }
    }
}
