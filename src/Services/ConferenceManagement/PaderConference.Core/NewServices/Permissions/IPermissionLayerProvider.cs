using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.NewServices.Permissions
{
    public interface IPermissionLayerProvider
    {
        ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(string conferenceId,
            string participantId);
    }
}
