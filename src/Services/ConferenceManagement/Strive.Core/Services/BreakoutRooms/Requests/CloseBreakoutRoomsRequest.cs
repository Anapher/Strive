using MediatR;

namespace Strive.Core.Services.BreakoutRooms.Requests
{
    public record CloseBreakoutRoomsRequest(string ConferenceId) : IRequest<Unit>;
}
