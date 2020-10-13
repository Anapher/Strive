using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public delegate ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsDelegate(Participant participant);
}