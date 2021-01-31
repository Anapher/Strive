using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.NewServices.Permissions.Dto;
using PaderConference.Core.Services;

namespace PaderConference.Core.NewServices.Permissions.Requests
{
    public record FetchPermissionsRequest
        (string? OfParticipant, ConferenceRequestMetadata Meta) : IRequest<SuccessOrError<ParticipantPermissionDto>>;
}
