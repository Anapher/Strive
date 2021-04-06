using Strive.Core.Services.Scenes;

namespace Strive.Hubs.Core.Dtos
{
    public record SetSceneDto(string RoomId, ActiveScene Active);
}
