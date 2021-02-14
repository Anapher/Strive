using PermissionsDict = System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JValue>;

namespace PaderConference.Core.Services.Permissions
{
    public record SynchronizedParticipantPermissions(PermissionsDict Permissions);
}
