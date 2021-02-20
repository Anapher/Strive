using System.Diagnostics.CodeAnalysis;

namespace PaderConference.Core.Services.Permissions
{
    public class DefinedPermissionValidator : IPermissionValidator
    {
        public virtual bool TryGetDescriptor(string permissionKey,
            [NotNullWhen(true)] out PermissionDescriptor? permissionDescriptor)
        {
            return DefinedPermissionsProvider.All.TryGetValue(permissionKey, out permissionDescriptor);
        }
    }
}
