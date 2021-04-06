using System.Diagnostics.CodeAnalysis;

namespace Strive.Core.Services.Permissions
{
    public interface IPermissionValidator
    {
        bool TryGetDescriptor(string permissionKey, [NotNullWhen(true)] out PermissionDescriptor? permissionDescriptor);
    }
}
