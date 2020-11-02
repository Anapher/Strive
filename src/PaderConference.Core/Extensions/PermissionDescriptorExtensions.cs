using System;
using System.Collections.Generic;
using System.Text.Json;
using PaderConference.Core.Services.Permissions;

namespace PaderConference.Core.Extensions
{
    public static class PermissionDescriptorExtensions
    {
        public static KeyValuePair<string, JsonElement> Configure<T>(this PermissionDescriptor<T> permissionDescriptor,
            T value)
        {
            return new KeyValuePair<string, JsonElement>(permissionDescriptor.Key,
                JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(value)));
        }

        public static bool ValidateValue(this PermissionDescriptor permissionDescriptor, JsonElement value)
        {
            switch (permissionDescriptor.Type)
            {
                case PermissionType.Boolean:
                    return value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False;
                case PermissionType.Integer:
                    return value.ValueKind == JsonValueKind.Number && value.GetDouble() % 1 < double.Epsilon;
                case PermissionType.Decimal:
                    return value.ValueKind == JsonValueKind.Number;
                case PermissionType.Text:
                    return value.ValueKind == JsonValueKind.String;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}