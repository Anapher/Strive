using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public interface IPermissionsService
    {
        ValueTask<IPermissionStack> GetPermissions(Participant participant);

        void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions);
    }
}