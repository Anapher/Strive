using System.Collections.Generic;
using MediatR;
using PermissionsDict = System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JValue>;

namespace PaderConference.Core.Services.Permissions.Notifications
{
    public record ParticipantPermissionsUpdatedNotification (
        Dictionary<Participant, PermissionsDict> UpdatedPermissions) : INotification;
}
