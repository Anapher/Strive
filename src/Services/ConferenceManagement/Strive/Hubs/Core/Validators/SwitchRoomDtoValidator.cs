using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class SwitchRoomDtoValidator : AbstractValidator<SwitchRoomDto>
    {
        public SwitchRoomDtoValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty();
        }
    }
}
