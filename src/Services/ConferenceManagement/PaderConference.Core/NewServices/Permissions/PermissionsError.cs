using PaderConference.Core.Dto;
using PaderConference.Core.Errors;
using PaderConference.Core.Services;

namespace PaderConference.Core.NewServices.Permissions
{
    public class PermissionsError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error PermissionKeyNotFound(string key)
        {
            return BadRequest($"The permission key \"{key}\" was not found.",
                ServiceErrorCode.Permissions_PermissionKeyNotFound);
        }

        public static Error InvalidPermissionValueType =>
            BadRequest("The type of the value does not match the permission definition.",
                ServiceErrorCode.Permissions_InvalidPermissionValueType);
    }
}
