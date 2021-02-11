using FluentValidation;
using PaderConference.Hubs.Dtos;

namespace PaderConference.Hubs.Validators
{
    public class SwitchRoomDtoValidator : AbstractValidator<SwitchRoomDto>
    {
        public SwitchRoomDtoValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty();
        }
    }
}
