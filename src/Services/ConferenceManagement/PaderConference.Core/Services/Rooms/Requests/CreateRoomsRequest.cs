using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Rooms.Requests
{
    public record CreateRoomsRequest
        (string ConferenceId, IReadOnlyList<RoomCreationInfo> Rooms) : IRequest<IReadOnlyList<Room>>;
}
