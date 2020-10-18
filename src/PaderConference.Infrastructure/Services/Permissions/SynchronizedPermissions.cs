using System.Collections.Immutable;
using System.Text.Json;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class SynchronizedPermissions
    {
        public SynchronizedPermissions(IImmutableDictionary<string, JsonElement> temporaryPermissions,
            IImmutableDictionary<string, JsonElement> moderatorPermissions,
            IImmutableDictionary<string, JsonElement> conferencePermissions)
        {
            TemporaryPermissions = temporaryPermissions;
            ModeratorPermissions = moderatorPermissions;
            ConferencePermissions = conferencePermissions;
        }

        public IImmutableDictionary<string, JsonElement> TemporaryPermissions { get; }

        public IImmutableDictionary<string, JsonElement> ModeratorPermissions { get; }

        public IImmutableDictionary<string, JsonElement> ConferencePermissions { get; }
    }
}