using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public interface IConferenceServiceManager
    {
        ValueTask Close(Conference conference);

        ValueTask<IConferenceService> GetService(Conference conference,
            IEnumerable<IConferenceServiceManager> services);
    }
}