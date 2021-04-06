using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;

namespace Strive.Core.Services.Scenes.UseCases
{
    public class FetchAvailableScenesUseCase : IRequestHandler<FetchAvailableScenesRequest, IReadOnlyList<IScene>>
    {
        private readonly ISceneRepository _sceneRepository;
        private readonly IEnumerable<ISceneProvider> _providers;

        public FetchAvailableScenesUseCase(ISceneRepository sceneRepository, IEnumerable<ISceneProvider> providers)
        {
            _sceneRepository = sceneRepository;
            _providers = providers;
        }

        public async Task<IReadOnlyList<IScene>> Handle(FetchAvailableScenesRequest request,
            CancellationToken cancellationToken)
        {
            var availableScenes = await _sceneRepository.GetAvailableScenes(request.ConferenceId, request.RoomId);
            if (availableScenes == null)
            {
                availableScenes = await GetAvailableScenes(request.ConferenceId, request.RoomId);
                await _sceneRepository.SetAvailableScenes(request.ConferenceId, request.RoomId, availableScenes);
            }

            return availableScenes;
        }

        private async ValueTask<IReadOnlyList<IScene>> GetAvailableScenes(string conferenceId, string roomId)
        {
            var result = new List<IScene>();

            foreach (var sceneProvider in _providers)
            {
                var scenes = await sceneProvider.GetAvailableScenes(conferenceId, roomId);
                result.AddRange(scenes);
            }

            return result;
        }
    }
}
