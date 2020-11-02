#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Permissions;

namespace PaderConference.Core.Tests.Services
{
    public class MockPermissionsService : IPermissionsService
    {
        private readonly Dictionary<string, IReadOnlyDictionary<string, JsonElement>> _permissions;

        public MockPermissionsService(Dictionary<string, IReadOnlyDictionary<string, JsonElement>> permissions)
        {
            _permissions = permissions;
        }

        public async ValueTask<IPermissionStack> GetPermissions(Participant participant)
        {
            if (_permissions.TryGetValue(participant.ParticipantId, out var permissions))
                return new PermissionStack(permissions.Yield().ToList());

            return new PermissionStack(new IReadOnlyDictionary<string, JsonElement>[0]);
        }

        public void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions)
        {
            throw new NotSupportedException();
        }

        public ValueTask UpdatePermissions(IEnumerable<Participant> participants)
        {
            throw new NotSupportedException();
        }
    }
}
