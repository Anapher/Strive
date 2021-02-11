using Newtonsoft.Json.Linq;

namespace PaderConference.Hubs.Dtos
{
    public record SetTemporaryPermissionDto(string ParticipantId, string PermissionKey, JValue? Value);
}
