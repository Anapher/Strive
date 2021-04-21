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
using Strive.Core.Services.Synchronization.Requests;

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
            if (notification.PreviousValue == null) return; // no update

            var conferenceId = notification.Participants.First().ConferenceId;
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId,
                SynchronizedRooms.SyncObjId);

            foreach (var room in rooms.Rooms)
            {
                var providersWithUpdate = await FetchScenesProvidersWithRequiredUpdate(conferenceId, room.RoomId,
                    notification.Value, notification.PreviousValue);

                if (providersWithUpdate.Any())
                {
                    await UpdateScenesInRoom(conferenceId, room.RoomId, providersWithUpdate);
                }
            }
        }

        private async ValueTask UpdateScenesInRoom(string conferenceId, string roomId,
            IReadOnlyList<ISceneProvider> updates)
        {
            await using (var @lock = await _sceneRepository.LockScene(conferenceId, roomId))
            {
                var sceneState = await _sceneRepository.GetSceneState(conferenceId, roomId);
                if (sceneState == null) return;

                IEnumerable<IScene> updatedScenes = sceneState.AvailableScenes;
                foreach (var provider in updates)
                {
                    var scenes = await provider.GetAvailableScenes(conferenceId, roomId, sceneState.SceneStack);
                    updatedScenes = ExcludeScenesProvidedBy(updatedScenes, provider).Concat(scenes);
                }

                var newAvailableScenes = updatedScenes.ToList();
                if (newAvailableScenes.ScrambledEquals(sceneState.AvailableScenes))
                {
                    return;
                }

                if (!sceneState.SceneStack.All(newAvailableScenes.Contains))
                {
                    await @lock.DisposeAsync();
                    await _mediator.Send(new UpdateScenesRequest(conferenceId, roomId));
                    return;
                }

                await _sceneRepository.SetSceneState(conferenceId, roomId,
                    sceneState with {AvailableScenes = newAvailableScenes});
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId,
                SynchronizedScene.SyncObjId(roomId)));
        }

        private async ValueTask<IReadOnlyList<ISceneProvider>> FetchScenesProvidersWithRequiredUpdate(
            string conferenceId, string roomId, object syncObj, object? previousSyncObj)
        {
            var result = new List<ISceneProvider>();
            foreach (var sceneProvider in _sceneProviders)
            {
                var updateRequired = await sceneProvider.IsUpdateRequired(conferenceId, roomId, syncObj,
                    previousSyncObj);

                if (updateRequired)
                    result.Add(sceneProvider);
            }

            return result;
        }

        private static IEnumerable<IScene> ExcludeScenesProvidedBy(IEnumerable<IScene> scenes, ISceneProvider provider)
        {
            return scenes.Where(x => !provider.IsProvided(x));
        }
    }
}
