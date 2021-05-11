using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using Strive.Core.Domain.Entities;
using Strive.Core.Extensions;

namespace Strive.Core.Services.Permissions.Options
{
    /// <summary>
    ///     Configuration options that define default permissions
    /// </summary>
    public class DefaultPermissionOptions
    {
        public Dictionary<PermissionType, IReadOnlyDictionary<string, JValue>> Default = new()
        {
            // Default conference permissions, so this is the foundation of permissions for every participant in the conference
            {
                PermissionType.Conference,
                new[]
                {
                    DefinedPermissions.Chat.CanSendChatMessage.Configure(true),
                    DefinedPermissions.Chat.CanSendAnonymously.Configure(true),
                    DefinedPermissions.Rooms.CanSwitchRoom.Configure(true),
                }.ToImmutableDictionary()
            },
            // Moderator permissions
            {
                PermissionType.Moderator,
                new[]
                {
                    DefinedPermissions.Conference.CanOpenAndClose.Configure(true),
                    DefinedPermissions.Conference.CanKickParticipant.Configure(true),
                    DefinedPermissions.Permissions.CanGiveTemporaryPermission.Configure(true),
                    DefinedPermissions.Permissions.CanSeeAnyParticipantsPermissions.Configure(true),
                    DefinedPermissions.Media.CanShareAudio.Configure(true),
                    DefinedPermissions.Media.CanShareScreen.Configure(true),
                    DefinedPermissions.Media.CanShareWebcam.Configure(true),
                    DefinedPermissions.Media.CanChangeOtherParticipantsProducers.Configure(true),
                    DefinedPermissions.Rooms.CanCreateAndRemove.Configure(true),
                    DefinedPermissions.Scenes.CanSetScene.Configure(true),
                    DefinedPermissions.Scenes.CanOverwriteContentScene.Configure(true),
                    DefinedPermissions.Chat.CanSendAnnouncement.Configure(true),
                    DefinedPermissions.Chat.CanSendAnonymously.Configure(false),
                    DefinedPermissions.Scenes.CanTakeTalkingStick.Configure(true),
                    DefinedPermissions.Scenes.CanPassTalkingStick.Configure(true),
                }.ToImmutableDictionary()
            },
            // Breakout room permissions
            {
                PermissionType.BreakoutRoom,
                new[]
                {
                    DefinedPermissions.Media.CanShareAudio.Configure(true),
                    DefinedPermissions.Media.CanShareScreen.Configure(true),
                    DefinedPermissions.Scenes.CanSetScene.Configure(true),
                    DefinedPermissions.Scenes.CanOverwriteContentScene.Configure(true),
                }.ToImmutableDictionary()
            },
        };
    }
}
