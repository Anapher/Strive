using MediatR;

namespace PaderConference.Core.Services.BreakoutRooms.Requests
{
    public record CloseBreakoutRoomsRequest(string ConferenceId) : IRequest<Unit>;
}
