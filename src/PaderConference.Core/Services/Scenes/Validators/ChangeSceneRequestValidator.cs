using FluentValidation;
using PaderConference.Core.Services.Scenes.Requests;

namespace PaderConference.Core.Services.Scenes.Validators
{
    public class ChangeSceneRequestValidator : AbstractValidator<ChangeSceneRequest>
    {
        public ChangeSceneRequestValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty();
            RuleFor(x => x.Scene).NotNull();
        }
    }
}
