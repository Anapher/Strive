using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class SetTemporaryPermissionDtoValidator : AbstractValidator<SetTemporaryPermissionDto>
    {
        public SetTemporaryPermissionDtoValidator()
        {
            RuleFor(x => x.ParticipantId).NotEmpty();
            RuleFor(x => x.PermissionKey).NotEmpty();
        }
    }
}
