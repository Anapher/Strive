using MediatR;

namespace Strive.Core.Services.Scenes.Requests
{
    public record SetOverwrittenContentSceneRequest
        (string ConferenceId, string RoomId, IScene? OverwrittenScene) : IRequest;
}
