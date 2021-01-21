using FluentValidation;
using PaderConference.Core.Services.Permissions.Requests;

namespace PaderConference.Core.Services.Permissions.Validation
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
