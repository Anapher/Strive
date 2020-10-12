using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Infrastructure.Extensions;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionsServiceManager : ConferenceServiceManager<PermissionsService>
    {
        protected override ValueTask<PermissionsService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            return new PermissionsService().ToValueTask();
        }
    }
}