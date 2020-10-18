using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services;
using PaderConference.Infrastructure.Services.Permissions.Dto;
using PaderConference.Infrastructure.Services.Rooms.Messages;

namespace PaderConference.Infrastructure
{
    public static class RedisChannels
    {
        /// <summary>
        ///     Reset all conferences, close existing ones and close all connections. No parameter
        /// </summary>
        public const string OnResetConferences = "resetConferences";

        /// <summary>
        ///     When the permissions updated (of a participant, conference, ...). The parameter is a
        ///     <see cref="PermissionUpdateDto" />
        /// </summary>
        public const string OnPermissionsUpdated = "permissionsUpdated";

        /// <summary>
        ///     Invoked once a conference was updated. The parameter is a <see cref="Conference" />
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        public static string OnConferenceUpdated(string conferenceId) => $"conferenceUpdated::{conferenceId}";

        /// <summary>
        ///     Invoked once a participant switched the room. The parameter is <see cref="ConnectionMessage{TPayload}" /> of
        ///     <see cref="RoomSwitchInfo" />
        /// </summary>
        public static string RoomSwitchedChannel(string conferenceId)
        {
            return $"{conferenceId}::roomSwitched";
        }
    }
}
