using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class UpdatePollStateDtoValidator : AbstractValidator<UpdatePollStateDto>
    {
        public UpdatePollStateDtoValidator()
        {
            RuleFor(x => x.PollId).NotEmpty();
            RuleFor(x => x.State).NotNull();
        }
    }
}
