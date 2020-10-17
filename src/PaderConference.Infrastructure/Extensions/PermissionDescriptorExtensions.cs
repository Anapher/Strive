using System.Collections.Generic;
using System.Text.Json;
using PaderConference.Infrastructure.Services.Permissions;

namespace PaderConference.Infrastructure.Extensions
{
    public static class PermissionDescriptorExtensions
    {
        public static KeyValuePair<string, JsonElement> Configure<T>(this PermissionDescriptor<T> permissionDescriptor,
            T value)
        {
            return new KeyValuePair<string, JsonElement>(permissionDescriptor.Key,
                JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(value)));
        }
    }
}