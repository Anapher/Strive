using MediatR;

namespace PaderConference.Core.Services.Scenes.Requests
{
    public record SetSceneRequest(string ConferenceId, string RoomId, ActiveScene Scene) : IRequest;
}
