using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class DeletePollDtoValidator : AbstractValidator<DeletePollDto>
    {
        public DeletePollDtoValidator()
        {
            RuleFor(x => x.PollId).NotEmpty();
        }
    }
}
