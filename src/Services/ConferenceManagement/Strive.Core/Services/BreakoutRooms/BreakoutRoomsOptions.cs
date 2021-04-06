using Strive.Core.Services.BreakoutRooms.Naming;

namespace Strive.Core.Services.BreakoutRooms
{
    public class BreakoutRoomsOptions
    {
        public IRoomNamingStrategy NamingStrategy { get; set; } = new NatoRoomNamingStrategy();
    }
}
