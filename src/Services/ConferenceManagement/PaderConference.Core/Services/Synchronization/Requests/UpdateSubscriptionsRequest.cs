using MediatR;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record UpdateSubscriptionsRequest(Participant Participant) : IRequest;
}
