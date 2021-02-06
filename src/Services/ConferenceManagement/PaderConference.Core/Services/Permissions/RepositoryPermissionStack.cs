using System.Threading.Tasks;
using PaderConference.Core.Services.Permissions.Gateways;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     A permission stack that retrieves the permissions from a repository (lazily)
    /// </summary>
    public class RepositoryPermissionStack : IPermissionStack
    {
        private readonly string _participantId;
        private readonly string _conferenceId;
        private readonly IAggregatedPermissionRepository _repo;

        public RepositoryPermissionStack(IAggregatedPermissionRepository repo, string participantId,
            string conferenceId)
        {
            _repo = repo;
            _participantId = participantId;
            _conferenceId = conferenceId;
        }

        public async ValueTask<T> GetPermissionValue<T>(PermissionDescriptor<T> descriptor)
        {
            return await _repo.GetPermissionsValue<T>(_conferenceId, _participantId, descriptor.Key) ??
                   (T) descriptor.DefaultValue;
        }
    }
}
