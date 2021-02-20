using MediatR;
using PaderConference.Core.Services.Permissions.Responses;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record FetchPermissionsRequest(Participant Participant) : IRequest<ParticipantPermissionResponse>;
}
