// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace PaderConference.Infrastructure.Services.Rooms
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

        public string RoomId { get; private set; }

        public string DisplayName { get; private set; }

        public bool IsEnabled { get; private set; }
    }
}