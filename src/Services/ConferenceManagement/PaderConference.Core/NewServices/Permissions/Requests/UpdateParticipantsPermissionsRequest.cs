using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.NewServices.Permissions.Requests
{
    public record UpdateParticipantsPermissionsRequest
        (string ConferenceId, IEnumerable<string> ParticipantIds) : IRequest;
}
