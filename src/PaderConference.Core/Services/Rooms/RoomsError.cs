using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services.Rooms
{
    public class RoomsError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error RoomNotFound(string roomId)
        {
            return NotFound($"The room {roomId} was not found.", ServiceErrorCode.Rooms_NotFound);
        }
    }
}