using FluentValidation;
using PaderConference.Hubs.Core.Dtos;

namespace PaderConference.Hubs.Core.Validators
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
