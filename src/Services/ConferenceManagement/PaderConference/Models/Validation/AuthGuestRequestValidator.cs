using FluentValidation;
using PaderConference.Models.Request;

namespace PaderConference.Models.Validation
{
    public class AuthGuestRequestValidator : AbstractValidator<AuthGuestRequestDto>
    {
        public AuthGuestRequestValidator()
        {
            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}