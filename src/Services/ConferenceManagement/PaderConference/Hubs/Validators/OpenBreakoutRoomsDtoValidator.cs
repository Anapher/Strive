using System;
using FluentValidation;
using PaderConference.Hubs.Dtos;

namespace PaderConference.Hubs.Validators
{
    public class OpenBreakoutRoomsDtoValidator : AbstractValidator<OpenBreakoutRoomsDto>
    {
        public OpenBreakoutRoomsDtoValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Deadline).GreaterThan(DateTimeOffset.UtcNow);
        }
    }
}
