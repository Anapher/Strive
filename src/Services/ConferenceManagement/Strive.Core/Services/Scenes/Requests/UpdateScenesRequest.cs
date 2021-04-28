using MediatR;

namespace Strive.Core.Services.Scenes.Requests
{
    public record UpdateScenesRequest(string ConferenceId, string RoomId) : IRequest;
}
