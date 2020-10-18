namespace PaderConference.Infrastructure.Services.Permissions
{
    public static class PermissionsList
    {
        public static class Conference
        {
            public static readonly PermissionDescriptor<bool> CanOpenAndClose =
                new PermissionDescriptor<bool>("conference.canOpenAndClose");

            public static readonly PermissionDescriptor<bool> CanGiveTemporaryPermission =
                new PermissionDescriptor<bool>("conference.canGiveTemporaryPermission");
        }

        public static class Chat
        {
            public static readonly PermissionDescriptor<bool> CanSendChatMessage =
                new PermissionDescriptor<bool>("chat.canSendMessage");

            public static readonly PermissionDescriptor<bool> CanSendPrivateChatMessage =
                new PermissionDescriptor<bool>("chat.canSendPrivateMessage");

            public static readonly PermissionDescriptor<bool> CanSendAnonymousMessage =
                new PermissionDescriptor<bool>("chat.canSendAnonymousMessage");
        }

        public static class Media
        {
            public static readonly PermissionDescriptor<bool> CanShareAudio =
                new PermissionDescriptor<bool>("media.canShareAudio");

            public static readonly PermissionDescriptor<bool> CanShareScreen =
                new PermissionDescriptor<bool>("media.canShareScreen");

            public static readonly PermissionDescriptor<bool> CanShareWebcam =
                new PermissionDescriptor<bool>("media.canShareWebcam");
        }

        public static class Rooms
        {
            public static readonly PermissionDescriptor<bool> CanSwitchRoom =
                new PermissionDescriptor<bool>("rooms.canSwitchRoom");
        }
    }
}