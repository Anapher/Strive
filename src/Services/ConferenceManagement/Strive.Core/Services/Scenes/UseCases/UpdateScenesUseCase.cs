using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Scenes.UseCases
{
    public class UpdateScenesUseCase : IRequestHandler<UpdateScenesRequest>
    {
        private static readonly ImmutableList<IScene> EmptySceneStack = ImmutableList<IScene>.Empty;

        private readonly IMediator _mediator;
        private readonly ISceneRepository _sceneRepository;
        private readonly IEnumerable<ISceneProvider> _sceneProviders;

        public UpdateScenesUseCase(IMediator mediator, ISceneRepository sceneRepository,
            IEnumerable<ISceneProvider> sceneProviders)
        {
            _mediator = mediator;
            _sceneRepository = sceneRepository;
            _sceneProviders = sceneProviders;
        }

        public async Task<Unit> Handle(UpdateScenesRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId) = request;

            await using (await _sceneRepository.LockScene(conferenceId, roomId))
            {
                var scene = await _sceneRepository.GetScene(conferenceId, roomId);
                scene ??= SynchronizedSceneProvider.GetDefaultActiveScene();

                var state = await _sceneRepository.GetSceneState(conferenceId, roomId);
                state ??= SynchronizedSceneProvider.GetEmptySceneState();

                await ApplyScene(conferenceId, roomId, scene, state);
            }

            await _mediator.Send(
                new UpdateSynchronizedObjectRequest(conferenceId, SynchronizedScene.SyncObjId(roomId)));

            return Unit.Value;
        }

        private async Task ApplyScene(string conferenceId, string roomId, ActiveScene scene, SceneState currentState)
        {
            var sceneState = await CreateNewSceneState(conferenceId, roomId, scene, currentState);
            await _sceneRepository.SetSceneState(conferenceId, roomId, sceneState);

            // optimistic concurrency

            // very important, we need to know the current state AFTER we saved the new state in repository
            await _mediator.Send(new ClearCacheRequest());

            var availableScenes =
                await _mediator.Send(new FetchAvailableScenesRequest(conferenceId, roomId, sceneState.SceneStack));

            if (!VerifySceneState(sceneState, availableScenes))
            {
                // something wrong with the applied scenes, we have to adjust here
                if (scene.OverwrittenContent != null && !availableScenes.Contains(scene.OverwrittenContent))
                {
                    scene = scene with {OverwrittenContent = null};
                }

                if (!availableScenes.Contains(scene.SelectedScene))
                {
                    scene = scene with
                    {
                        SelectedScene = scene.OverwrittenContent ?? SynchronizedSceneProvider.GetDefaultScene(),
                        OverwrittenContent = null,
                    };
                }

                await _sceneRepository.SetScene(conferenceId, roomId, scene);
                await ApplyScene(conferenceId, roomId, scene, sceneState);
            }
        }

        private async Task<SceneState> CreateNewSceneState(string conferenceId, string roomId, ActiveScene scene,
            SceneState currentState)
        {
            var availableScenes =
                await _mediator.Send(new FetchAvailableScenesRequest(conferenceId, roomId, EmptySceneStack));
            var stack = await BuildSceneStack(conferenceId, roomId, scene, currentState, availableScenes);

            availableScenes = await _mediator.Send(new FetchAvailableScenesRequest(conferenceId, roomId, stack));

            return new SceneState(stack, availableScenes);
        }

        private async Task<IReadOnlyList<IScene>> BuildSceneStack(string conferenceId, string roomId,
            ActiveScene activeScene, SceneState currentState, IReadOnlyList<IScene> availableScenes)
        {
            var (selectedScene, _) = activeScene;

            var conference = await _mediator.Send(new FindConferenceByIdRequest(conferenceId));

            var context = new SceneBuilderContext(conferenceId, roomId, availableScenes,
                conference.Configuration.Scenes, currentState.SceneStack);

            ValueTask<IEnumerable<IScene>> SceneStackFunc(IScene scene, SceneBuilderContext builderContext)
            {
                return FindProviderForScene(scene).BuildStack(scene, builderContext, SceneStackFunc);
            }

            var stack = await SceneStackFunc(selectedScene, context);

            var result = stack.ToList();
            if (activeScene.OverwrittenContent != null)
                result.Add(activeScene.OverwrittenContent);

            return result;
        }

        private ISceneProvider FindProviderForScene(IScene scene)
        {
            return _sceneProviders.First(x => x.IsProvided(scene));
        }

        private static bool VerifySceneState(SceneState state, IReadOnlyList<IScene> availableScenes)
        {
            var (stack, savedAvailableScenes) = state;
            return stack.All(availableScenes.Contains) && availableScenes.SequenceEqual(savedAvailableScenes);
        }
    }
}
