using PaderConference.Core.Dto;

namespace PaderConference.Core.Services.Scenes
{
    public class SceneError
    {
        public static Error PermissionDeniedToChangeScene =>
            new ServiceError("You don't have permissions to change the current scene.",
                ServiceErrorCode.Scenes_PermissionDenied_Change);

        public static Error RoomNotFound =>
            new ServiceError("The room the scene should be changed in was not found.",
                ServiceErrorCode.Scenes_RoomNotFound);

        public static Error SceneMustNotBeNull =>
            new ServiceError("The new scene must not be null.", ServiceErrorCode.Scenes_SceneMustNotBeNull);
    }
}
