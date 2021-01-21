using FluentValidation;
using PaderConference.Core.Services.BreakoutRoom.Dto;

namespace PaderConference.Core.Services.BreakoutRoom.Validation
{
    public class BreakoutRoomsOptionsValidator : AbstractValidator<BreakoutRoomsOptions>
    {
        public BreakoutRoomsOptionsValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Cannot create zero or less breakout rooms.");
        }
    }
}
