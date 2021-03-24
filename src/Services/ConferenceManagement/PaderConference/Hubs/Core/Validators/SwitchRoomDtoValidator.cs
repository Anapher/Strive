using FluentValidation;
using PaderConference.Hubs.Core.Dtos;

namespace PaderConference.Hubs.Core.Validators
{
    public class SwitchRoomDtoValidator : AbstractValidator<SwitchRoomDto>
    {
        public SwitchRoomDtoValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty();
        }
    }
}
