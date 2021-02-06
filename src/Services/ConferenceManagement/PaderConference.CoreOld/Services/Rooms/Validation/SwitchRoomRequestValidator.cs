using FluentValidation;
using PaderConference.Core.Services.Rooms.Requests;

namespace PaderConference.Core.Services.Rooms.Validation
{
    public class SwitchRoomRequestValidator : AbstractValidator<SwitchRoomRequest>
    {
        public SwitchRoomRequestValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty();
        }
    }
}
