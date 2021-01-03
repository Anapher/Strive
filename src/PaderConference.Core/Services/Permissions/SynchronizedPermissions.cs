using System.Collections.Immutable;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Services.Permissions
{
    public class SynchronizedPermissions
    {
        public SynchronizedPermissions(IImmutableDictionary<string, JValue> temporaryPermissions,
            IImmutableDictionary<string, JValue> moderatorPermissions,
            IImmutableDictionary<string, JValue> conferencePermissions)
        {
            TemporaryPermissions = temporaryPermissions;
            ModeratorPermissions = moderatorPermissions;
            ConferencePermissions = conferencePermissions;
        }

        public IImmutableDictionary<string, JValue> TemporaryPermissions { get; }

        public IImmutableDictionary<string, JValue> ModeratorPermissions { get; }

        public IImmutableDictionary<string, JValue> ConferencePermissions { get; }
    }
}