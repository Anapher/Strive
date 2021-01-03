using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record SetTemporaryPermissionRequest(string ParticipantId, string PermissionKey, JValue? Value);
}
