using System.Threading.Tasks;
using Strive.Core.Services.Permissions.Gateways;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Permissions
{
    public class
        SynchronizedTemporaryPermissionsProvider : SynchronizedObjectProviderForAll<SynchronizedTemporaryPermissions>
    {
        private readonly ITemporaryPermissionRepository _repository;

        public SynchronizedTemporaryPermissionsProvider(ITemporaryPermissionRepository repository)
        {
            _repository = repository;
        }

        public override string Id { get; } = SynchronizedTemporaryPermissions.SyncObjId.Id;

        protected override async ValueTask<SynchronizedTemporaryPermissions> InternalFetchValue(string conferenceId)
        {
            var temporaryPermissions = await _repository.FetchConferenceTemporaryPermissions(conferenceId);
            return new SynchronizedTemporaryPermissions(temporaryPermissions);
        }
    }
}
