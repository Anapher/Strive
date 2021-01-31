using FluentValidation;
using PaderConference.Core.NewServices.Permissions.Requests;

namespace PaderConference.Core.NewServices.Permissions.Validation
{
    public class SetTemporaryPermissionRequestValidator : AbstractValidator<SetTemporaryPermissionRequest>
    {
        public SetTemporaryPermissionRequestValidator()
        {
            RuleFor(x => x.ParticipantId).NotEmpty();
            RuleFor(x => x.PermissionKey).NotEmpty();
        }
    }
}
