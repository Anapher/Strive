using FluentValidation;
using PaderConference.Hubs.Core.Dtos;

namespace PaderConference.Hubs.Core.Validators
{
    public class SetSceneDtoValidator : AbstractValidator<SetSceneDto>
    {
        public SetSceneDtoValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty();
            RuleFor(x => x.Active).NotNull();
        }
    }
}
