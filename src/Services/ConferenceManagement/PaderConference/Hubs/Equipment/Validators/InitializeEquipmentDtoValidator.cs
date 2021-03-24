using FluentValidation;
using PaderConference.Hubs.Equipment.Dtos;

namespace PaderConference.Hubs.Equipment.Validators
{
    public class InitializeEquipmentDtoValidator : AbstractValidator<InitializeEquipmentDto>
    {
        public InitializeEquipmentDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
