using MediatR;
using Newtonsoft.Json.Linq;
using Strive.Core.Interfaces;

namespace Strive.Core.Services.Permissions.Requests
{
    public record SetTemporaryPermissionRequest
        (Participant Participant, string PermissionKey, JValue? Value) : IRequest<SuccessOrError<Unit>>;
}
