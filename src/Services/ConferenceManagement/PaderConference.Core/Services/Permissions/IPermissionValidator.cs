using System.Diagnostics.CodeAnalysis;

namespace PaderConference.Core.Services.Permissions
{
    public interface IPermissionValidator
    {
        bool TryGetDescriptor(string permissionKey, [NotNullWhen(true)] out PermissionDescriptor? permissionDescriptor);
    }
}
