using PaderConference.Core.Dto;

namespace PaderConference.Core.Services.Permissions
{
    public static class PermissionsError
    {
        public static Error PermissionKeyNotFound =>
            new ServiceError("The permission key was not found.", ServiceErrorCode.Permissions_PermissionKeyNotFound);

        public static Error PermissionDeniedGiveTemporaryPermission =>
            new ServiceError("Permission denied to give temporary permission.",
                ServiceErrorCode.Permissions_DeniedGiveTemporaryPermission);

        public static Error InvalidPermissionValueType =>
            new ServiceError("The type of the value does not match the permission definition.",
                ServiceErrorCode.Permissions_InvalidPermissionValueType);

        public static Error ParticipantNotFound =>
            new ServiceError("The participant was not found in this conference.",
                ServiceErrorCode.Permissions_ParticipantNotFound);
    }
}
