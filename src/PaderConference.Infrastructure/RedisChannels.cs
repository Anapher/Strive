using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services.Permissions.Dto;

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
        public static string OnConferenceUpdated(string conferenceId)
        {
            return $"conferenceUpdated::{conferenceId}";
        }
    }
}