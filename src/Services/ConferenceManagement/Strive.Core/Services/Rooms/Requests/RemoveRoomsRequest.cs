using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Rooms.Requests
{
    public record RemoveRoomsRequest(string ConferenceId, IReadOnlyList<string> RoomIds) : IRequest;
}
