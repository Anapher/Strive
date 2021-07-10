using Strive.Core.Dto;
using Strive.Core.Errors;

namespace Strive.Core.Services.Scenes
{
    public class SceneError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error AlreadyHasSpeaker =>
            NotFound("The room already has a speaker.", ServiceErrorCode.Scenes_HasSpeaker);
    }
}
