using PaderConference.Core.Services.BreakoutRooms.Naming;

namespace PaderConference.Core.Services.BreakoutRooms
{
    public class BreakoutRoomsOptions
    {
        public IRoomNamingStrategy NamingStrategy { get; set; } = new NatoRoomNamingStrategy();
    }
}
