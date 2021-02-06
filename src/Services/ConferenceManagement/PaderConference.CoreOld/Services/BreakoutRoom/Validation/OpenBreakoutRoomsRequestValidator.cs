using FluentValidation;
using PaderConference.Core.Services.BreakoutRoom.Requests;

namespace PaderConference.Core.Services.BreakoutRoom.Validation
{
    public class OpenBreakoutRoomsRequestValidator : AbstractValidator<OpenBreakoutRoomsRequest>
    {
        public OpenBreakoutRoomsRequestValidator()
        {
            Include(new BreakoutRoomsOptionsValidator());
            RuleFor(x => x.AssignedRooms).Must((context, value) => value == null || context.Amount >= value.Length)
                .WithMessage(
                    "The amount of assigned rooms must not be greater than the actual amount of rooms created.");
        }
    }
}
