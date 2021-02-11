using FluentValidation;
using PaderConference.Hubs.Dtos;

namespace PaderConference.Hubs.Validators
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
