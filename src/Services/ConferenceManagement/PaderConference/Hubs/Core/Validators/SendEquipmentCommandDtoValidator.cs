using FluentValidation;
using PaderConference.Hubs.Core.Dtos;

namespace PaderConference.Hubs.Core.Validators
{
    public class SendEquipmentCommandDtoValidator : AbstractValidator<SendEquipmentCommandDto>
    {
        public SendEquipmentCommandDtoValidator()
        {
            RuleFor(x => x.Action).IsInEnum();
            RuleFor(x => x.Source).IsInEnum();
            RuleFor(x => x.ConnectionId).NotEmpty();
        }
    }
}
