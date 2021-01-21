using System.Collections.Generic;

namespace PaderConference.Core.Services.Permissions.Dto
{
    public record ParticipantPermissionInfo(string ParticipantId, IEnumerable<PermissionLayer> Layers);
}
