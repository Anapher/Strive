using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Scenes.Utilities;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Scenes.Providers
{
    public class PresenterSceneProvider : ISceneProvider
    {
        private static readonly IReadOnlyDictionary<string, JValue> PresenterPermissions = new[]
        {
            DefinedPermissions.Media.CanShareAudio.Configure(true),
            DefinedPermissions.Media.CanShareWebcam.Configure(true),
            DefinedPermissions.Media.CanShareScreen.Configure(true),
            DefinedPermissions.Scenes.CanOverwriteContentScene.Configure(true),
        }.ToImmutableDictionary();

        private readonly IMediator _mediator;

        public PresenterSceneProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            if (sceneStack.OfType<PresenterScene>().Any())
            {
                var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId,
                    SynchronizedRooms.SyncObjId);

                return GetAvailableScenes(roomId, rooms, sceneStack);
            }

            return Enumerable.Empty<IScene>();
        }

        public async ValueTask<bool> IsUpdateRequired(string conferenceId, string roomId, object synchronizedObject,
            object? previousValue)
        {
            if (synchronizedObject is SynchronizedRooms rooms)
            {
                if (SceneUtilities.ParticipantsOfRoomChanged(roomId, rooms, previousValue as SynchronizedRooms))
                    return true;
            }

            return false;
        }

        private static IEnumerable<IScene> GetAvailableScenes(string roomId, SynchronizedRooms rooms,
            IReadOnlyList<IScene> scenes)
        {
            var presenterScenes = scenes.OfType<PresenterScene>();

            return presenterScenes.Where(x =>
                rooms.Participants.TryGetValue(x.PresenterParticipantId, out var presenterRoomId) &&
                presenterRoomId == roomId);
        }

        public bool IsProvided(IScene scene)
        {
            return scene is PresenterScene;
        }

        public async ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            SceneStackFunc sceneProviderFunc)
        {
            var presenterScene = (PresenterScene) scene;
            var stack = new List<IScene> {presenterScene};

            var screenShare = context.AvailableScenes.OfType<ScreenShareScene>()
                .FirstOrDefault(x => x.ParticipantId == presenterScene.PresenterParticipantId);
            if (screenShare != null)
            {
                stack.Add(screenShare);
            }
            else
            {
                stack.Add(ActiveSpeakerScene.Instance);
            }

            return stack;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene,
            Participant participant, IReadOnlyList<IScene> sceneStack)
        {
            if (scene is PresenterScene presenterScene && presenterScene.PresenterParticipantId == participant.Id)
            {
                return new List<PermissionLayer> {CommonPermissionLayers.ScenePresenter(PresenterPermissions)};
            }

            return Enumerable.Empty<PermissionLayer>();
        }
    }
}
