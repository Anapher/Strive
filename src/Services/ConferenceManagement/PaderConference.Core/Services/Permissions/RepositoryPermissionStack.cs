using System.Threading.Tasks;
using PaderConference.Core.Services.Permissions.Gateways;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     A permission stack that retrieves the permissions from a repository (lazily)
    /// </summary>
    public class RepositoryPermissionStack : IPermissionStack
    {
        private readonly IAggregatedPermissionRepository _repo;
        private readonly Participant _participant;

        public RepositoryPermissionStack(IAggregatedPermissionRepository repo, Participant participant)
        {
            _repo = repo;
            _participant = participant;
        }

        public async ValueTask<T> GetPermissionValue<T>(PermissionDescriptor<T> descriptor)
        {
            return await _repo.GetPermissionsValue<T>(_participant, descriptor.Key) ?? (T) descriptor.DefaultValue;
        }
    }
}
