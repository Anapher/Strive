using System.Collections.Generic;

namespace PaderConference.Core.NewServices.Permissions.Dto
{
    public record ParticipantPermissionDto(string ParticipantId, IEnumerable<PermissionLayer> Layers);
}
