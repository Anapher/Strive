using System.Text.Json;

namespace PaderConference.Infrastructure.Services.Permissions.Messages
{
    public class SetTemporaryPermissionMessage
    {
        public SetTemporaryPermissionMessage(string participantId, string permissionKey, JsonElement? value)
        {
            ParticipantId = participantId;
            PermissionKey = permissionKey;
            Value = value;
        }

        public string ParticipantId { get; }

        public string PermissionKey { get; }

        public JsonElement? Value { get; }
    }
}
