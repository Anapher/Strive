using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Poll.Requests
{
    public record FetchPollsOfRoomRequest(string ConferenceId, string RoomId) : IRequest<IReadOnlyList<Poll>>;
}
