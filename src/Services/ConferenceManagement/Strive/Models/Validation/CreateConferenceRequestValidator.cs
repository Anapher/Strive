using FluentValidation;
using Strive.Models.Request;

namespace Strive.Models.Validation
{
    public class CreateConferenceRequestValidator : AbstractValidator<CreateConferenceRequestDto>
    {
        public CreateConferenceRequestValidator()
        {
        }
    }
}