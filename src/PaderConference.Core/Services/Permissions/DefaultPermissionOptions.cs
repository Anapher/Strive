using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     Configuration options that define default permissions
    /// </summary>
    public class DefaultPermissionOptions
    {
        public Dictionary<PermissionType, IReadOnlyDictionary<string, JsonElement>> Default = new()
        {
            // Default conference permissions, so this is the foundation of permissions for every participant in the conference
            {
                PermissionType.Conference,
                new[]
                {
                    PermissionsList.Chat.CanSendChatMessage.Configure(true),
                    PermissionsList.Chat.CanSendAnonymousMessage.Configure(true),
                    PermissionsList.Chat.CanSendPrivateChatMessage.Configure(true),
                    PermissionsList.Rooms.CanSwitchRoom.Configure(true),
                    PermissionsList.Conference.CanRaiseHand.Configure(true),
                }.ToImmutableDictionary()
            },
            // Moderator permissions
            {
                PermissionType.Moderator,
                new[]
                {
                    PermissionsList.Conference.CanOpenAndClose.Configure(true),
                    PermissionsList.Conference.CanKickParticipant.Configure(true),
                    PermissionsList.Permissions.CanGiveTemporaryPermission.Configure(true),
                    PermissionsList.Permissions.CanSeeAnyParticipantsPermissions.Configure(true),
                    PermissionsList.Media.CanShareAudio.Configure(true),
                    PermissionsList.Media.CanShareScreen.Configure(true),
                    PermissionsList.Media.CanShareWebcam.Configure(true),
                    PermissionsList.Media.CanChangeOtherParticipantsProducers.Configure(true),
                    PermissionsList.Rooms.CanCreateAndRemove.Configure(true),
                    PermissionsList.Scenes.CanSetScene.Configure(true),
                }.ToImmutableDictionary()
            },
            // Moderator permissions
            {
                PermissionType.BreakoutRoom,
                new[]
                {
                    PermissionsList.Media.CanShareAudio.Configure(true),
                    PermissionsList.Media.CanShareScreen.Configure(true),
                }.ToImmutableDictionary()
            },
        };
    }
}
