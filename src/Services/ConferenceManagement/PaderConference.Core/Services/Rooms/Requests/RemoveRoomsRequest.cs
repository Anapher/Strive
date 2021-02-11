using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Rooms.Requests
{
    public record RemoveRoomsRequest(string ConferenceId, IReadOnlyList<string> RoomIds) : IRequest;
}
