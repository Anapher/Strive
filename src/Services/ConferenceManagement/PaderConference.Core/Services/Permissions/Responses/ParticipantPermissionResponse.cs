using System.Collections.Generic;

namespace PaderConference.Core.Services.Permissions.Responses
{
    public record ParticipantPermissionResponse(string ParticipantId, IEnumerable<PermissionLayer> Layers);
}
