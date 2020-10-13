using System.Collections.Generic;
using System.Text.Json;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionLayer
    {
        public PermissionLayer(int order, IReadOnlyDictionary<string, JsonElement> permissions)
        {
            Order = order;
            Permissions = permissions;
        }

        public int Order { get; }

        public IReadOnlyDictionary<string, JsonElement> Permissions { get; }
    }
}