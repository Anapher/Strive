using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class SubmitPollAnswerDtoValidator : AbstractValidator<SubmitPollAnswerDto>
    {
        public SubmitPollAnswerDtoValidator()
        {
            RuleFor(x => x.PollId).NotEmpty();
            RuleFor(x => x.Answer).NotNull();
        }
    }
}
