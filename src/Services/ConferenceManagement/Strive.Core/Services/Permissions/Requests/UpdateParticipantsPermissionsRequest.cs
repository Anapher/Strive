using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Permissions.Requests
{
    public record UpdateParticipantsPermissionsRequest (IEnumerable<Participant> Participants) : IRequest;
}
