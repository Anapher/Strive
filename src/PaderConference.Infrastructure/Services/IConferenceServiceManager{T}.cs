using System.Collections.Generic;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public interface IConferenceServiceManager<out TService> : IConferenceServiceManager
        where TService : IConferenceService
    {
        new TService GetService(Conference conference, IEnumerable<IConferenceServiceManager> services);
    }
}