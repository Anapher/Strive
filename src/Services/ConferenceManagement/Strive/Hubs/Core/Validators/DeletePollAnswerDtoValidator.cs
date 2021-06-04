using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class DeletePollAnswerDtoValidator : AbstractValidator<DeletePollAnswerDto>
    {
        public DeletePollAnswerDtoValidator()
        {
            RuleFor(x => x.PollId).NotEmpty();
        }
    }
}
