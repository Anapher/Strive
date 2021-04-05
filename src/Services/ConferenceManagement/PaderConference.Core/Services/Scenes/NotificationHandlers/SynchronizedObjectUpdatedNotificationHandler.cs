using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Scenes.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Scenes.NotificationHandlers
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
            var rooms = await FetchSynchronizedRooms(conferenceId);

            foreach (var room in rooms.Rooms)
            {
                var updates = new List<(IEnumerable<IScene>, ISceneProvider)>();

                foreach (var sceneProvider in _sceneProviders)
                {
                    var update =
                        await sceneProvider.UpdateAvailableScenes(conferenceId, room.RoomId, notification.Value);
                    if (update.Required)
                        updates.Add((update.UpdateScenes, sceneProvider));
                }

                if (updates.Any())
                {
                    var availableScenes = await _sceneRepository.GetAvailableScenes(conferenceId, room.RoomId);
                    if (availableScenes == null) continue;

                    IEnumerable<IScene> updatedScenes = availableScenes;
                    foreach (var (update, provider) in updates)
                    {
                        updatedScenes = ExcludeScenesProvidedBy(updatedScenes, provider).Concat(update);
                    }

                    var newScenes = updatedScenes.ToList();
                    if (newScenes.ScrambledEquals(availableScenes)) continue;

                    await _sceneRepository.SetAvailableScenes(conferenceId, room.RoomId, newScenes);
                    await OnAvailableScenesUpdated(conferenceId, room.RoomId, newScenes);

                    await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId,
                        SynchronizedScene.SyncObjId(room.RoomId)));
                }
            }
        }

        private async ValueTask OnAvailableScenesUpdated(string conferenceId, string roomId,
            IReadOnlyList<IScene> scenes)
        {
            var scene = await _sceneRepository.GetScene(conferenceId, roomId);
            if (scene == null) return;
            if (scenes.Contains(scene.Scene)) return;

            await _sceneRepository.SetScene(conferenceId, roomId, scene with {Scene = null});
        }

        private async ValueTask<SynchronizedRooms> FetchSynchronizedRooms(string conferenceId)
        {
            return (SynchronizedRooms) await _mediator.Send(
                new FetchSynchronizedObjectRequest(conferenceId, SynchronizedRooms.SyncObjId));
        }

        private static IEnumerable<IScene> ExcludeScenesProvidedBy(IEnumerable<IScene> scenes, ISceneProvider provider)
        {
            return scenes.Where(x => !provider.IsProvided(x));
        }
    }
}
