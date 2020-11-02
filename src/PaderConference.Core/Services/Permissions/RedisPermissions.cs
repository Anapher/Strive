using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Core.Services.Permissions
{
    public class RedisPermissions : IPermissionStack
    {
        private readonly string _participantId;
        private readonly IPermissionsRepo _repo;

        public RedisPermissions(IPermissionsRepo repo, string participantId)
        {
            _repo = repo;
            _participantId = participantId;
        }

        public async ValueTask<T> GetPermission<T>(PermissionDescriptor<T> descriptor)
        {
            return await _repo.GetPermissionsValue<T>(_participantId, descriptor.Key) ?? (T) descriptor.DefaultValue;
        }
    }
}
