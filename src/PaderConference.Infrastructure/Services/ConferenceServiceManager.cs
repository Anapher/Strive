using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public abstract class ConferenceServiceManager<TService> : IConferenceServiceManager<TService>
        where TService : IConferenceService
    {
        private readonly ConcurrentDictionary<Conference, TService> _services =
            new ConcurrentDictionary<Conference, TService>();

        public ValueTask Close(Conference conference)
        {
            if (_services.TryRemove(conference, out var service))
                return service.DisposeAsync();

            return new ValueTask();
        }

        IConferenceService IConferenceServiceManager.GetService(Conference conference,
            IEnumerable<IConferenceServiceManager> services)
        {
            return GetService(conference, services);
        }

        public TService GetService(Conference conference, IEnumerable<IConferenceServiceManager> services)
        {
            if (!_services.TryGetValue(conference, out var service))
                return _services.GetOrAdd(conference, ServiceFactory, services);

            return service;
        }

        protected abstract TService ServiceFactory(Conference conference,
            IEnumerable<IConferenceServiceManager> services);
    }
}