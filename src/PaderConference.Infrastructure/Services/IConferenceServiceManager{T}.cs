using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public interface IConferenceServiceManager<TService> : IConferenceServiceManager
        where TService : IConferenceService
    {
        new ValueTask<TService> GetService(Conference conference, IEnumerable<IConferenceServiceManager> services);
    }
}