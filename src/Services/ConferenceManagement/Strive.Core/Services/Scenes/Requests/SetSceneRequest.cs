using MediatR;

namespace Strive.Core.Services.Scenes.Requests
{
    public record SetSceneRequest(string ConferenceId, string RoomId, IScene Scene) : IRequest;
}
