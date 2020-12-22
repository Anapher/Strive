using System.Text.Json;

namespace PaderConference.Core.Services.Permissions.Dto
{
    public record SetTemporaryPermissionDto
    {
        public string? ParticipantId { get; init; }
        public string? PermissionKey { get; init; }
        public JsonElement? Value { get; init; }

        public void Deconstruct(out string? participantId, out string? permissionKey, out JsonElement? value)
        {
            participantId = ParticipantId;
            permissionKey = PermissionKey;
            value = Value;
        }
    }
}
