using MediatR;
using Strive.Core.Services.Permissions.Responses;

namespace Strive.Core.Services.Permissions.Requests
{
    public record FetchPermissionsRequest(Participant Participant) : IRequest<ParticipantPermissionResponse>;
}
