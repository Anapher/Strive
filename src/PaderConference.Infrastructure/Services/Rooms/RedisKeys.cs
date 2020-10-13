namespace PaderConference.Infrastructure.Services.Rooms
{
    public static class RedisKeys
    {
        public static string ParticipantsToRoom(string conferenceId)
        {
            return $"{conferenceId}::participantToRoom";
        }

        public static string RoomPermissions(string conferenceId, string roomId)
        {
            return $"{conferenceId}::roomPermissions/{roomId}";
        }

        public static string Rooms(string conferenceId)
        {
            return $"{conferenceId}::rooms";
        }

        public static string RoomSwitchedChannel(string conferenceId)
        {
            return $"{conferenceId}::roomSwitched";
        }

        public static string GetDefaultRoomId(string conferenceId)
        {
            return $"{conferenceId}::defaultRoomId";
        }
    }
}