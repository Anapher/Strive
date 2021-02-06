using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;

namespace PaderConference.Core.Services.Permissions.Options
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
                    DefinedPermissions.Chat.CanSendAnonymousMessage.Configure(true),
                    DefinedPermissions.Chat.CanSendPrivateChatMessage.Configure(true),
                    DefinedPermissions.Rooms.CanSwitchRoom.Configure(true),
                    DefinedPermissions.Conference.CanRaiseHand.Configure(true),
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
                }.ToImmutableDictionary()
            },
            // Moderator permissions
            {
                PermissionType.BreakoutRoom,
                new[]
                {
                    DefinedPermissions.Media.CanShareAudio.Configure(true),
                    DefinedPermissions.Media.CanShareScreen.Configure(true),
                }.ToImmutableDictionary()
            },
        };
    }
}
