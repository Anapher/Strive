using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Rooms.Requests
{
    public record CreateRoomsRequest
        (string ConferenceId, IReadOnlyList<RoomCreationInfo> Rooms) : IRequest<IReadOnlyList<Room>>;
}
