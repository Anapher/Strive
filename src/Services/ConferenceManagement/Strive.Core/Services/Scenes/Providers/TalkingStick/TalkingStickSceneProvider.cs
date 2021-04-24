using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick
{
    public class TalkingStickSceneProvider : ISceneProvider
    {
        private readonly IMediator _mediator;

        private static readonly IImmutableDictionary<string, JValue> DenyMediaPermissions = new[]
        {
            DefinedPermissions.Media.CanShareAudio.Configure(false),
            DefinedPermissions.Media.CanShareWebcam.Configure(false),
            DefinedPermissions.Media.CanShareScreen.Configure(false),
        }.ToImmutableDictionary();

        private static readonly IImmutableDictionary<string, JValue> DenyMediaPermissionsButCanQueue =
            DenyMediaPermissions.SetItems(DefinedPermissions.Scenes.CanQueueForTalkingStick.Configure(true).Yield());

        private static readonly IReadOnlyDictionary<TalkingStickMode, TalkingStickModePermissions> Permissions =
            new Dictionary<TalkingStickMode, TalkingStickModePermissions>
            {
                {
                    TalkingStickMode.Queue, new TalkingStickModePermissions
                    {
                        Others = DenyMediaPermissionsButCanQueue,
                        OthersNoCurrentSpeaker = DenyMediaPermissionsButCanQueue,
                    }
                },
                {
                    TalkingStickMode.Race, new TalkingStickModePermissions
                    {
                        Others = DenyMediaPermissions,
                        OthersNoCurrentSpeaker =
                            DenyMediaPermissions.SetItems(DefinedPermissions.Scenes.CanTakeTalkingStick.Configure(true)
                                .Yield()),
                    }
                },
                {
                    TalkingStickMode.Moderated, new TalkingStickModePermissions
                    {
                        Others = DenyMediaPermissionsButCanQueue,
                        OthersNoCurrentSpeaker = DenyMediaPermissionsButCanQueue,
                    }
                },
                {
                    TalkingStickMode.SpeakerPassStick, new TalkingStickModePermissions
                    {
                        Others = DenyMediaPermissionsButCanQueue,
                        OthersNoCurrentSpeaker = DenyMediaPermissionsButCanQueue,
                        CurrentSpeaker = DefinedPermissions.Scenes.CanPassTalkingStick.Configure(true).Yield()
                            .ToImmutableDictionary(),
                    }
                },
            };

        public TalkingStickSceneProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            return sceneStack.OfType<TalkingStickScene>();
        }

        public async ValueTask<bool> IsUpdateRequired(string conferenceId, string roomId, object synchronizedObject,
            object? previousValue)
        {
            if (synchronizedObject is SynchronizedSceneTalkingStick talkingStick)
            {
                var previous = previousValue as SynchronizedSceneTalkingStick;
                return talkingStick.CurrentSpeakerId != previous?.CurrentSpeakerId;
            }

            return false;
        }

        public bool IsProvided(IScene scene)
        {
            return scene is TalkingStickScene;
        }

        public async ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            SceneStackFunc sceneProviderFunc)
        {
            var stack = new List<IScene> {scene};

            var syncObj = await GetSyncObj(context.ConferenceId, context.RoomId);
            if (syncObj.CurrentSpeakerId != null)
            {
                var presenterScene = new PresenterScene(syncObj.CurrentSpeakerId);
                stack.AddRange(await sceneProviderFunc(presenterScene, context));
            }

            return stack;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene,
            Participant participant, IReadOnlyList<IScene> sceneStack)
        {
            if (scene is TalkingStickScene talkingStickScene)
            {
                var presenterScene = sceneStack.SkipWhile(x => x is not TalkingStickScene).Skip(1)
                    .OfType<PresenterScene>().LastOrDefault();

                var permissions = GetPermissions(participant, presenterScene, talkingStickScene);
                return CommonPermissionLayers.ScenePassTheStone(permissions).Yield();
            }

            return Enumerable.Empty<PermissionLayer>();
        }

        private IReadOnlyDictionary<string, JValue> GetPermissions(Participant participant,
            PresenterScene? presenterScene, TalkingStickScene scene)
        {
            var isPresenter = participant.Id == presenterScene?.PresenterParticipantId;
            var hasPresenter = presenterScene != null;

            var permissions = Permissions[scene.Mode];

            if (isPresenter)
                return permissions.CurrentSpeaker;

            if (hasPresenter)
                return permissions.Others;

            return permissions.OthersNoCurrentSpeaker;
        }

        private async ValueTask<SynchronizedSceneTalkingStick> GetSyncObj(string conferenceId, string roomId)
        {
            return await _mediator.FetchSynchronizedObject<SynchronizedSceneTalkingStick>(conferenceId,
                SynchronizedSceneTalkingStick.SyncObjId(roomId));
        }

        private class TalkingStickModePermissions
        {
            public IReadOnlyDictionary<string, JValue> CurrentSpeaker { get; init; } =
                ImmutableDictionary<string, JValue>.Empty;

            public IReadOnlyDictionary<string, JValue> Others { get; init; } =
                ImmutableDictionary<string, JValue>.Empty;

            public IReadOnlyDictionary<string, JValue> OthersNoCurrentSpeaker { get; init; } =
                ImmutableDictionary<string, JValue>.Empty;
        }
    }
}
