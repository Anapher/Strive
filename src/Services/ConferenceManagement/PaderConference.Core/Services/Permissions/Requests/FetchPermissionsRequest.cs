using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Permissions.Dto;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record FetchPermissionsRequest
        (string ParticipantId, string ConferenceId) : IRequest<SuccessOrError<ParticipantPermissionDto>>;
}
