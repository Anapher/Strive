using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services.Permissions
{
    public delegate ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsDelegate(Participant participant);
}