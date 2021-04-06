using System.Collections.Generic;
using System.Threading.Tasks;

namespace Strive.Core.Services.Permissions
{
    public interface IPermissionLayerProvider
    {
        ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(Participant participant);
    }
}
