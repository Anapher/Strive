using MediatR;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick.Requests
{
    public record TalkingStickEnqueueRequest(Participant Participant, bool Remove) : IRequest;
}
