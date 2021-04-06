using FluentValidation;
using Strive.Hubs.Equipment.Dtos;

namespace Strive.Hubs.Equipment.Validators
{
    public class InitializeEquipmentDtoValidator : AbstractValidator<InitializeEquipmentDto>
    {
        public InitializeEquipmentDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
