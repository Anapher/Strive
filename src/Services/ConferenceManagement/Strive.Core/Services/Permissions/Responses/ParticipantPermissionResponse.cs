using System.Collections.Generic;

namespace Strive.Core.Services.Permissions.Responses
{
    public record ParticipantPermissionResponse(string ParticipantId, IEnumerable<PermissionLayer> Layers);
}
