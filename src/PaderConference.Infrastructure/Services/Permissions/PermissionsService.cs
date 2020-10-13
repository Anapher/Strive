using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Extensions;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionsService : ConferenceService, IPermissionsService
    {
        private readonly ConcurrentBag<FetchPermissionsDelegate> _fetchPermissionsDelegates =
            new ConcurrentBag<FetchPermissionsDelegate>();

        public PermissionsService()
        {
            ConferencePermissions = ImmutableDictionary<string, JsonElement>.Empty;
        }

        public IImmutableDictionary<string, JsonElement> ConferencePermissions { get; }

        public async ValueTask<IPermissionStack> GetPermissions(Participant participant)
        {
            var layers = new List<PermissionLayer>();
            foreach (var fetchPermissionsDelegate in _fetchPermissionsDelegates)
                layers.AddRange(await fetchPermissionsDelegate(participant));

            return new PermissionStack(layers.OrderBy(x => x.Order).Select(x => x.Permissions).ToList());
        }

        public void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions)
        {
            _fetchPermissionsDelegates.Add(fetchPermissions);
        }

        public override ValueTask InitializeAsync()
        {
            RegisterLayerProvider(FetchConferencePermissions);

            return new ValueTask();
        }

        private ValueTask<IEnumerable<PermissionLayer>> FetchConferencePermissions(Participant participant)
        {
            return new ValueTask<IEnumerable<PermissionLayer>>(new PermissionLayer(20, ConferencePermissions).Yield());
        }
    }
}