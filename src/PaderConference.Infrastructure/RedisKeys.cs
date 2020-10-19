using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services.Media.Communication;
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

            /// <summary>
            ///     The default room id of the conference. String.
            /// </summary>
            public static string DefaultRoomId(string conferenceId)
            {
                return $"{conferenceId}::defaultRoomId";
            }
        }

        public static class Media
        {
            /// <summary>
            ///     The key where the new conferences (List of <see cref="ConferenceInfo" />) are stored
            /// </summary>
            public const string NewConferencesKey = "newConferences";

            /// <summary>
            ///     The key where the rtc capabilities of the router are stored, encoded in JSON
            /// </summary>
            public static readonly ConferenceDependentKey RtpCapabilitiesKey =
                new ConferenceDependentKey("::routerRtpCapabilities");
        }
    }
}
