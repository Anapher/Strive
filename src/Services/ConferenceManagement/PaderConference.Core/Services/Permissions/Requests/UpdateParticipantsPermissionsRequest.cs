using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record UpdateParticipantsPermissionsRequest
        (string ConferenceId, IEnumerable<string> ParticipantIds) : IRequest;
}
