using System.Collections.Generic;
using MediatR;
using PermissionsDict = System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JValue>;

namespace PaderConference.Core.NewServices.Permissions.Notifications
{
    public record ParticipantPermissionsUpdatedNotification (string ConferenceId,
        Dictionary<string, PermissionsDict> UpdatedPermissions) : INotification;
}
