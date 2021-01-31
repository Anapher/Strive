using MediatR;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;

namespace PaderConference.Core.NewServices.Permissions.Requests
{
    public record SetTemporaryPermissionRequest(string ParticipantId, string PermissionKey, JValue? Value,
        ConferenceRequestMetadata Meta) : IRequest<SuccessOrError>;
}
