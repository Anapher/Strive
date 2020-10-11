using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Extensions;

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

        async ValueTask<IConferenceService> IConferenceServiceManager.GetService(Conference conference,
            IEnumerable<IConferenceServiceManager> services)
        {
            return await GetService(conference, services);
        }

        public async ValueTask<TService> GetService(Conference conference,
            IEnumerable<IConferenceServiceManager> services)
        {
            if (!_services.TryGetValue(conference, out var service))
            {
                service = await ServiceFactory(conference, services);
                var actualService = _services.GetOrAdd(conference, service);

                if (ReferenceEquals(service, actualService))
                    await service.InitializeAsync();
                else service.DisposeAsync().Forget();

                return actualService;
            }

            return service;
        }

        protected abstract ValueTask<TService> ServiceFactory(Conference conference,
            IEnumerable<IConferenceServiceManager> services);
    }
}