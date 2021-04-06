using System;
using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
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
