using Newtonsoft.Json.Linq;

namespace Strive.Hubs.Core.Dtos
{
    public record SetTemporaryPermissionDto(string ParticipantId, string PermissionKey, JValue? Value);
}
