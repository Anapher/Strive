using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record UpdateParticipantsPermissionsRequest (IEnumerable<Participant> Participants) : IRequest;
}
