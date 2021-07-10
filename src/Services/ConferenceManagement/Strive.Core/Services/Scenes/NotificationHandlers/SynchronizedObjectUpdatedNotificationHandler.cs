using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Synchronization.Extensions;
using Strive.Core.Services.Synchronization.Notifications;

namespace Strive.Core.Services.Scenes.NotificationHandlers
{
    public class
        SynchronizedObjectUpdatedNotificationHandler : INotificationHandler<SynchronizedObjectUpdatedNotification>
    {
        private readonly IMediator _mediator;
        private readonly IEnumerable<ISceneProvider> _sceneProviders;
        private readonly ISceneRepository _sceneRepository;

        public SynchronizedObjectUpdatedNotificationHandler(IMediator mediator,
            IEnumerable<ISceneProvider> sceneProviders, ISceneRepository sceneRepository)
        {
            _mediator = mediator;
            _sceneProviders = sceneProviders;
            _sceneRepository = sceneRepository;
        }

        public async Task Handle(SynchronizedObjectUpdatedNotification notification,
            CancellationToken cancellationToken)
        {
            var conferenceId = notification.Participants.First().ConferenceId;
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId,
                SynchronizedRooms.SyncObjId);

            foreach (var room in rooms.Rooms)
            {
                var providersWithUpdate = await FetchSceneProvidersWithRequiredUpdate(conferenceId, room.RoomId,
                    notification.SyncObjId, notification.Value, notification.PreviousValue);

                if (providersWithUpdate.Any())
                {
                    await UpdateScenesInRoom(conferenceId, room.RoomId, providersWithUpdate);
                }
            }
        }

        private async ValueTask<IReadOnlyList<(ISceneProvider, SceneUpdate)>> FetchSceneProvidersWithRequiredUpdate(
            string conferenceId, string roomId, string syncObjId, object syncObj, object? previousSyncObj)
        {
            var result = new List<(ISceneProvider, SceneUpdate)>();
            foreach (var sceneProvider in _sceneProviders)
            {
                var sceneUpdate =
                    await sceneProvider.IsUpdateRequired(conferenceId, roomId, syncObjId, syncObj, previousSyncObj);

                if (sceneUpdate != SceneUpdate.NotRequired)
                    result.Add((sceneProvider, sceneUpdate));
            }

            return result;
        }

        private async ValueTask UpdateScenesInRoom(string conferenceId, string roomId,
            IReadOnlyList<(ISceneProvider, SceneUpdate)> updatedProviders)
        {
            var sceneState = await _sceneRepository.GetSceneState(conferenceId, roomId);
            if (sceneState == null) return;

            if (updatedProviders.All(x => x.Item2 == SceneUpdate.AvailableScenesChanged))
            {
                IEnumerable<IScene> updatedScenes = sceneState.AvailableScenes;
                foreach (var (provider, _) in updatedProviders)
                {
                    var scenes = await provider.GetAvailableScenes(conferenceId, roomId, sceneState.SceneStack);
                    updatedScenes = ExcludeScenesProvidedBy(updatedScenes, provider).Concat(scenes);
                }

                var newAvailableScenes = updatedScenes.ToList();
                if (newAvailableScenes.ScrambledEquals(sceneState.AvailableScenes))
                    return;
            }

            await _mediator.Send(new UpdateScenesRequest(conferenceId, roomId));
        }

        private static IEnumerable<IScene> ExcludeScenesProvidedBy(IEnumerable<IScene> scenes, ISceneProvider provider)
        {
            return scenes.Where(x => !provider.IsProvided(x));
        }
    }
}
