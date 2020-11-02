using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services.Permissions
{
    public interface IPermissionsService
    {
        ValueTask<IPermissionStack> GetPermissions(Participant participant);

        void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions);

        ValueTask UpdatePermissions(IEnumerable<Participant> participants);
    }
}