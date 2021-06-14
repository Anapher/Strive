namespace Strive.Core.Services
{
    public static class SynchronizedObjectIds
    {
        // global/for all
        public const string ROOMS = "rooms";
        public const string CONFERENCE = "conference";
        public const string PARTICIPANTS = "participants";
        public const string CHAT = "chat";
        public const string BREAKOUT_ROOMS = "breakoutRooms";
        public const string TEMPORARY_PERMISSIONS = "temporaryPermissions";
        public const string POLL = "poll";
        public const string POLL_RESULT = "poll_result";
        public const string POLL_ANSWERS = "poll_answers";

        // individual
        public const string MEDIA = "media";
        public const string EQUIPMENT = "equipment";
        public const string SUBSCRIPTIONS = "subscriptions";
        public const string PARTICIPANT_PERMISSIONS = "participantPermissions";

        // room
        public const string SCENE = "scene";
        public const string SCENE_TALKINGSTICK = "scene_talkingStick";
        public const string WHITEBOARDS = "whiteboards";
    }
}
