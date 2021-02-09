using MediatR;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Services.Permissions.Requests
{
    public record SetTemporaryPermissionRequest(string ParticipantId, string PermissionKey, JValue? Value,
        string ConferenceId) : IRequest<SuccessOrError<Unit>>;
}
