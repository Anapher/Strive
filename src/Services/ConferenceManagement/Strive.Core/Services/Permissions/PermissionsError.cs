using Strive.Core.Dto;
using Strive.Core.Errors;

namespace Strive.Core.Services.Permissions
{
    public class PermissionsError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error InvalidPermissionValueType =>
            BadRequest("The type of the value does not match the permission definition.",
                ServiceErrorCode.Permissions_InvalidPermissionValueType);

        public static Error PermissionKeyNotFound(string key)
        {
            return BadRequest($"The permission key \"{key}\" was not found.",
                ServiceErrorCode.Permissions_PermissionKeyNotFound);
        }
    }
}
