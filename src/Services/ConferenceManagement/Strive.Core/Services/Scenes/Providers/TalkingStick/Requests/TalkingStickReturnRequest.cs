using MediatR;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick.Requests
{
    public record TalkingStickReturnRequest(Participant Participant) : IRequest;
}
