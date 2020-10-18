using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public static class PermissionsListUtil
    {
        static PermissionsListUtil()
        {
            var result = new Dictionary<string, PermissionDescriptor>();

            var permissionClasses = typeof(PermissionsList).GetNestedTypes().Concat(new[] {typeof(PermissionsList)});
            foreach (var permissionClass in permissionClasses)
            foreach (var fieldInfo in permissionClass.GetFields(BindingFlags.Static | BindingFlags.Public))
                if (typeof(PermissionDescriptor).IsAssignableFrom(fieldInfo.FieldType))
                {
                    var descriptor = (PermissionDescriptor?) fieldInfo.GetValue(null);
                    if (descriptor != null)
                        result.Add(descriptor.Key, descriptor);
                    ;
                }

            All = result.ToImmutableDictionary();
        }

        public static IImmutableDictionary<string, PermissionDescriptor> All { get; }
    }
}
