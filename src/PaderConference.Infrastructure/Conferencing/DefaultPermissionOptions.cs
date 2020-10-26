using System.Collections.Immutable;
using System.Text.Json;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Services.Permissions;

namespace PaderConference.Infrastructure.Conferencing
{
    public class DefaultPermissionOptions
    {
        public ImmutableDictionary<string, JsonElement> Conference { get; set; } = new[]
        {
            PermissionsList.Chat.CanSendChatMessage.Configure(true),
            PermissionsList.Rooms.CanSwitchRoom.Configure(true),
            PermissionsList.Conference.CanRaiseHand.Configure(true),
        }.ToImmutableDictionary();

        public ImmutableDictionary<string, JsonElement> Moderator { get; set; } = new[]
        {
            PermissionsList.Conference.CanOpenAndClose.Configure(true),
            PermissionsList.Conference.CanGiveTemporaryPermission.Configure(true),
            PermissionsList.Media.CanShareAudio.Configure(true),
            PermissionsList.Media.CanShareScreen.Configure(true),
            PermissionsList.Media.CanShareWebcam.Configure(true),
            PermissionsList.Rooms.CanCreateAndRemove.Configure(true),
        }.ToImmutableDictionary();

        public ImmutableDictionary<string, JsonElement> Room { get; set; } = new[]
        {
            PermissionsList.Media.CanShareAudio.Configure(true),
            PermissionsList.Media.CanShareScreen.Configure(true),
        }.ToImmutableDictionary();
    }
}
