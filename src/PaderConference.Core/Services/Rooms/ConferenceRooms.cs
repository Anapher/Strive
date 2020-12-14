using System.Collections.Immutable;

namespace PaderConference.Core.Services.Rooms
{
    public record ConferenceRooms
    {
        public IImmutableList<Room> Rooms { get; init; } = ImmutableList<Room>.Empty;
        public string DefaultRoomId { get; init; } = RoomOptions.DEFAULT_ROOM_ID;

        public IImmutableDictionary<string, string> Participants { get; init; } =
            ImmutableDictionary<string, string>.Empty;
    }
}