using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     This delegate defines a method that fetches permissions layers for a participant
    /// </summary>
    /// <param name="participant">The participant</param>
    /// <returns>Return permission layers that should be applied for the participant</returns>
    public delegate ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsDelegate(Participant participant);
}