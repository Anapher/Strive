using System.Collections.Generic;

namespace PaderConference.Core.Services.Permissions.Dto
{
    public record ParticipantPermissionDto(string ParticipantId, IEnumerable<PermissionLayer> Layers);
}
