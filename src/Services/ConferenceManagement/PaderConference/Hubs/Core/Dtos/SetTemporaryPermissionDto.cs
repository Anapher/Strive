using Newtonsoft.Json.Linq;

namespace PaderConference.Hubs.Core.Dtos
{
    public record SetTemporaryPermissionDto(string ParticipantId, string PermissionKey, JValue? Value);
}
