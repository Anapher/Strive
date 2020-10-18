using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services.Rooms;

namespace PaderConference.Infrastructure
{
    public static class RedisKeys
    {
        /// <summary>
        ///     Hashmap: conference id => <see cref="Conference" />
        /// </summary>
        public const string OpenConferences = "openConferences";

        /// <summary>
        ///     Hashmap: participant id => json value
        /// </summary>
        public static string ParticipantPermissions(string participantId)
        {
            return $"participantPermissions::{participantId}";
        }

        public static class Rooms
        {
            /// <summary>
            ///     Hashmap: participant id -> room id
            /// </summary>
            public static string ParticipantsToRoom(string conferenceId)
            {
                return $"{conferenceId}::participantToRoom";
            }

            /// <summary>
            ///     Hashmap: roomid -> <see cref="Room" />
            /// </summary>
            public static string RoomList(string conferenceId)
            {
                return $"{conferenceId}::rooms";
            }

            public static string RoomPermissions(string conferenceId, string roomId)
            {
                return $"{conferenceId}::roomPermissions/{roomId}";
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
}
