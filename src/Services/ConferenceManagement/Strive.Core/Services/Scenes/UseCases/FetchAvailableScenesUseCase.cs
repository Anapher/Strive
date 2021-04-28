using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Scenes.Requests;

namespace Strive.Core.Services.Scenes.UseCases
{
    public class FetchAvailableScenesUseCase : IRequestHandler<FetchAvailableScenesRequest, IReadOnlyList<IScene>>
    {
        private readonly IEnumerable<ISceneProvider> _providers;

        public FetchAvailableScenesUseCase(IEnumerable<ISceneProvider> providers)
        {
            _providers = providers;
        }

        public async Task<IReadOnlyList<IScene>> Handle(FetchAvailableScenesRequest request,
            CancellationToken cancellationToken)
        {
            var result = new List<IScene>();

            foreach (var sceneProvider in _providers)
            {
                var scenes =
                    await sceneProvider.GetAvailableScenes(request.ConferenceId, request.RoomId, request.SceneStack);
                result.AddRange(scenes);
            }

            return result;
        }
    }
}
