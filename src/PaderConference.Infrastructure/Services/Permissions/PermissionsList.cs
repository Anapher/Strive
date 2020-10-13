namespace PaderConference.Infrastructure.Services.Permissions
{
    public static class PermissionsList
    {
        public static class Chat
        {
            public static readonly PermissionDescriptor<bool> CanSendChatMessage =
                new PermissionDescriptor<bool>("media.canSendMessage");

            public static readonly PermissionDescriptor<bool> CanSendPrivateChatMessage =
                new PermissionDescriptor<bool>("media.canSendPrivateMessage");

            public static readonly PermissionDescriptor<bool> CanSendAnonymousMessage =
                new PermissionDescriptor<bool>("media.canSendAnonymousMessage");
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
    }
}