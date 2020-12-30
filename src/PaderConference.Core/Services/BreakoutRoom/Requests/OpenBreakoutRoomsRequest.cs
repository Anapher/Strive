using PaderConference.Core.Services.BreakoutRoom.Dto;

namespace PaderConference.Core.Services.BreakoutRoom.Requests
{
    public class OpenBreakoutRoomsRequest : BreakoutRoomsOptions
    {
        public string[][]? AssignedRooms { get; set; }
    }
}
