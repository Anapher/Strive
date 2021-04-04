using PaderConference.Core.Services.Scenes;

namespace PaderConference.Hubs.Core.Dtos
{
    public record SetSceneDto(string RoomId, ActiveScene Active);
}
