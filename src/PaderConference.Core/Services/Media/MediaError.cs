using PaderConference.Core.Dto;

namespace PaderConference.Core.Services.Media
{
    public static class MediaError
    {
        public static Error PermissionToChangeOtherParticipantsProducersDenied =>
            new ServiceError("The permissions to change producers of another participant were denied.",
                ServiceErrorCode.Media_PermissionToChangeOtherParticipantsProducersDenied);
    }
}
