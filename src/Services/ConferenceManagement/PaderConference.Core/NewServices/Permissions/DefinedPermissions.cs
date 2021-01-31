namespace PaderConference.Core.NewServices.Permissions
{
    public static class DefinedPermissions
    {
        public static class Conference
        {
            public static readonly PermissionDescriptor<bool> CanOpenAndClose = new("conference/canOpenAndClose");
            public static readonly PermissionDescriptor<bool> CanRaiseHand = new("conference/canRaiseHand");
            public static readonly PermissionDescriptor<bool> CanKickParticipant = new("conference/canKickParticipant");
        }

        public static class Permissions
        {
            public static readonly PermissionDescriptor<bool> CanGiveTemporaryPermission =
                new("permissions/canGiveTemporaryPermission");

            public static readonly PermissionDescriptor<bool> CanSeeAnyParticipantsPermissions =
                new("permissions/canSeeAnyParticipantsPermissions");
        }

        public static class Chat
        {
            public static readonly PermissionDescriptor<bool> CanSendChatMessage = new("chat/canSendMessage");

            public static readonly PermissionDescriptor<bool> CanSendPrivateChatMessage =
                new("chat/canSendPrivateMessage");

            public static readonly PermissionDescriptor<bool> CanSendAnonymousMessage =
                new("chat/canSendAnonymousMessage");
        }

        public static class Media
        {
            public static readonly PermissionDescriptor<bool> CanShareAudio = new("media/canShareAudio");
            public static readonly PermissionDescriptor<bool> CanShareScreen = new("media/canShareScreen");
            public static readonly PermissionDescriptor<bool> CanShareWebcam = new("media/canShareWebcam");

            public static readonly PermissionDescriptor<bool> CanChangeOtherParticipantsProducers =
                new("media/canChangeOtherParticipantsProducers");
        }

        public static class Rooms
        {
            public static readonly PermissionDescriptor<bool> CanCreateAndRemove = new("rooms/canCreateAndRemove");
            public static readonly PermissionDescriptor<bool> CanSwitchRoom = new("rooms/canSwitchRoom");
        }

        public static class Scenes
        {
            public static readonly PermissionDescriptor<bool> CanSetScene = new("scenes/canSetScene");
        }
    }
}
