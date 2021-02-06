#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Permissions;

namespace PaderConference.Core.Tests.Services
{
    public class MockPermissionsService : IPermissionsService
    {
        private readonly Dictionary<string, IReadOnlyDictionary<string, JValue>> _permissions;

        public MockPermissionsService(Dictionary<string, IReadOnlyDictionary<string, JValue>> permissions)
        {
            _permissions = permissions;
        }

        public async ValueTask<IPermissionStack> GetPermissions(Participant participant)
        {
            if (_permissions.TryGetValue(participant.ParticipantId, out var permissions))
                return new CachedPermissionStack(permissions.Yield().ToList());

            return new CachedPermissionStack(new IReadOnlyDictionary<string, JValue>[0]);
        }

        public void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions)
        {
            throw new NotSupportedException();
        }

        public ValueTask RefreshPermissions(IEnumerable<Participant> participants)
        {
            throw new NotSupportedException();
        }
    }
}
