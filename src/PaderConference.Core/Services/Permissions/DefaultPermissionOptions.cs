using System.Collections.Immutable;
using System.Text.Json;
using PaderConference.Core.Extensions;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     Configuration options that define default permissions
    /// </summary>
    public class DefaultPermissionOptions
    {
        /// <summary>
        ///     Default conference permissions, so this is the foundation of permissions for every participant in the
        ///     conference
        /// </summary>
        public ImmutableDictionary<string, JsonElement> Conference { get; set; } = new[]
        {
            PermissionsList.Chat.CanSendChatMessage.Configure(true),
            PermissionsList.Chat.CanSendAnonymousMessage.Configure(true),
            PermissionsList.Chat.CanSendPrivateChatMessage.Configure(true),
            PermissionsList.Rooms.CanSwitchRoom.Configure(true),
            PermissionsList.Conference.CanRaiseHand.Configure(true),
        }.ToImmutableDictionary();

        /// <summary>
        ///     Moderator permissions
        /// </summary>
        public ImmutableDictionary<string, JsonElement> Moderator { get; set; } = new[]
        {
            PermissionsList.Conference.CanOpenAndClose.Configure(true),
            PermissionsList.Permissions.CanGiveTemporaryPermission.Configure(true),
            PermissionsList.Permissions.CanSeeAnyParticipantsPermissions.Configure(true),
            PermissionsList.Media.CanShareAudio.Configure(true),
            PermissionsList.Media.CanShareScreen.Configure(true),
            PermissionsList.Media.CanShareWebcam.Configure(true),
            PermissionsList.Rooms.CanCreateAndRemove.Configure(true),
            PermissionsList.Scenes.CanSetScene.Configure(true),
        }.ToImmutableDictionary();

        /// <summary>
        ///     Permissions participants in breakout rooms have
        /// </summary>
        public ImmutableDictionary<string, JsonElement> BreakoutRoom { get; set; } = new[]
        {
            PermissionsList.Media.CanShareAudio.Configure(true),
            PermissionsList.Media.CanShareScreen.Configure(true),
        }.ToImmutableDictionary();
    }
}
