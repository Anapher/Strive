// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace PaderConference.Core.Services.Rooms
{
    public class Room
    {
        public Room(string roomId, string displayName, bool isEnabled)
        {
            RoomId = roomId;
            DisplayName = displayName;
            IsEnabled = isEnabled;
        }

#pragma warning disable 8618
        private Room()
        {
        }
#pragma warning restore 8618

        public string RoomId { get; set; }

        public string DisplayName { get; set; }

        public bool IsEnabled { get; set; }
    }
}