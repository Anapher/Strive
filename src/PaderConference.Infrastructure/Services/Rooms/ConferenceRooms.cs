using System.Collections.Immutable;

namespace PaderConference.Infrastructure.Services.Rooms
{
    public class ConferenceRooms
    {
        public ConferenceRooms(IImmutableList<Room> rooms, string defaultRoomId,
            IImmutableDictionary<string, string> participants)
        {
            Rooms = rooms;
            DefaultRoomId = defaultRoomId;
            Participants = participants;
        }

        public IImmutableList<Room> Rooms { get; }

        public string DefaultRoomId { get; }

        public IImmutableDictionary<string, string> Participants { get; }
    }
}