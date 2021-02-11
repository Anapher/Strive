using MediatR;
using PaderConference.Core.Services.Permissions.Responses;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record FetchPermissionsRequest
        (string ParticipantId, string ConferenceId) : IRequest<ParticipantPermissionResponse>;
}
