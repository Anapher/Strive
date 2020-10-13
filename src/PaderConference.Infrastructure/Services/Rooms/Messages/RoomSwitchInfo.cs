namespace PaderConference.Infrastructure.Services.Rooms.Messages
{
    public class RoomSwitchInfo
    {
        public RoomSwitchInfo(string? previousRoom, string newRoom)
        {
            PreviousRoom = previousRoom;
            NewRoom = newRoom;
        }

        public string? PreviousRoom { get; }

        public string NewRoom { get; }
    }
}