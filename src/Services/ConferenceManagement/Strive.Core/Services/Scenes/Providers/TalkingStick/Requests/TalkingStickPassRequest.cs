using MediatR;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick.Requests
{
    public record TalkingStickPassRequest(Participant Participant, string RoomId) : IRequest;
}
