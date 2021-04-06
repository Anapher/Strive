using MediatR;

namespace Strive.Core.Services.Media.Requests
{
    public record FetchSfuConnectionInfoRequest
        (Participant Participant, string ConnectionId) : IRequest<SfuConnectionInfo>;
}
