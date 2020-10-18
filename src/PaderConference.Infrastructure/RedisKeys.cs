using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services.Permissions;

namespace PaderConference.Infrastructure
{
    public static class RedisKeys
    {
        /// <summary>
        ///     Hashmap: conference id => <see cref="Conference" />
        /// </summary>
        public const string OpenConferences = "openConferences";

        /// <summary>
        ///     Hashmap: <see cref="ParticipantPermissionKey" /> => json value
        /// </summary>
        public static string ParticipantPermissions(string participantId)
        {
            return $"participantPermissions::{participantId}";
        }
    }
}
