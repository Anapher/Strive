using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     A permission stack that retrieves the permissions from a repository (lazily)
    /// </summary>
    public class RepositoryPermissions : IPermissionStack
    {
        private readonly string _participantId;
        private readonly IPermissionsRepo _repo;

        /// <summary>
        ///     Initialize a new instance of <see cref="RepositoryPermissions" />
        /// </summary>
        /// <param name="repo">The permission repository</param>
        /// <param name="participantId">The participant id the permissions should be retrieved from</param>
        public RepositoryPermissions(IPermissionsRepo repo, string participantId)
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
