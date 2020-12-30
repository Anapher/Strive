using System.Text.Json;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record SetTemporaryPermissionRequest(string ParticipantId, string PermissionKey, JsonElement? Value);
}
