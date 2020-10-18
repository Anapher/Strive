// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace PaderConference.Infrastructure.Services.Rooms.Messages
{
    public class CreateRoomMessage
    {
        public CreateRoomMessage(string displayName)
        {
            DisplayName = displayName;
        }

#pragma warning disable 8618
        private CreateRoomMessage()
        {
        }
#pragma warning restore 8618

        public string DisplayName { get; set; }
    }
}
