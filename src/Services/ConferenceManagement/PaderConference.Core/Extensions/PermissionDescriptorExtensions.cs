using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services.Permissions;

namespace PaderConference.Core.Extensions
{
    public static class PermissionDescriptorExtensions
    {
        public static KeyValuePair<string, JValue> Configure<T>(this PermissionDescriptor<T> permissionDescriptor,
            T value) where T : notnull
        {
            return new(permissionDescriptor.Key, (JValue) JToken.FromObject(value));
        }

        public static bool ValidateValue(this PermissionDescriptor permissionDescriptor, JValue value)
        {
            switch (permissionDescriptor.Type)
            {
                case PermissionValueType.Boolean:
                    return value.Type == JTokenType.Boolean;
                case PermissionValueType.Integer:
                    return value.Type == JTokenType.Integer;
                case PermissionValueType.Decimal:
                    return value.Type == JTokenType.Float;
                case PermissionValueType.Text:
                    return value.Type == JTokenType.String;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}