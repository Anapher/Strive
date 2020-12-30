using FluentValidation;
using PaderConference.Models.Request;

namespace PaderConference.Models.Validation
{
    public class CreateConferenceRequestValidator : AbstractValidator<CreateConferenceRequestDto>
    {
        public CreateConferenceRequestValidator()
        {
        }
    }
}