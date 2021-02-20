using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Permissions
{
    public interface IPermissionLayerProvider
    {
        ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(Participant participant);
    }
}
