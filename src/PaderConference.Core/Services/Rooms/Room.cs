// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace PaderConference.Core.Services.Rooms
{
    public class Room
    {
        public Room(string roomId, string displayName)
        {
            RoomId = roomId;
            DisplayName = displayName;
        }

#pragma warning disable 8618
        private Room()
        {
        }
#pragma warning restore 8618

        public string RoomId { get; set; }

        public string DisplayName { get; set; }
    }
}