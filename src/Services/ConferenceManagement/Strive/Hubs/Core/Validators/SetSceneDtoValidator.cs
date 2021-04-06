using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
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
