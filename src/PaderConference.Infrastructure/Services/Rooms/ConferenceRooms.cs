using System.Collections.Immutable;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

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

        public IImmutableList<Room> Rooms { get; private set; }

        public string DefaultRoomId { get; private set; }

        public IImmutableDictionary<string, string> Participants { get; private set; }
    }
}