using MediatR;

namespace Strive.Core.Services.Synchronization.Requests
{
    public record UpdateSubscriptionsRequest(Participant Participant) : IRequest;
}
