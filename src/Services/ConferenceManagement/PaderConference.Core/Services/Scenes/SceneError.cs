using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services.Scenes
{
    public class SceneError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error RoomNotFound =>
            NotFound("The room the scene should be set for was not found.", ServiceErrorCode.Scenes_RoomNotFound);

        public static Error InvalidScene =>
            NotFound("The scene is invalid for this room.", ServiceErrorCode.Scenes_Invalid);
    }
}
