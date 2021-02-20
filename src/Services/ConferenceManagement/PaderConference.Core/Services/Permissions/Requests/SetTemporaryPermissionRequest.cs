using MediatR;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record SetTemporaryPermissionRequest
        (Participant Participant, string PermissionKey, JValue? Value) : IRequest<SuccessOrError<Unit>>;
}
