using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services.Scenes
{
    public class SceneError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error RoomNotFound =>
            NotFound("The room the scene should be changed in was not found.", ServiceErrorCode.Scenes_RoomNotFound);
    }
}
