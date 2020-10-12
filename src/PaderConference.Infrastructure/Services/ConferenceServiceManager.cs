using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Infrastructure.Extensions;

namespace PaderConference.Infrastructure.Services
{
    public abstract class ConferenceServiceManager<TService> : IConferenceServiceManager<TService>
        where TService : IConferenceService
    {
        private readonly ConcurrentDictionary<string, TService> _services =
            new ConcurrentDictionary<string, TService>();

        public ValueTask Close(string conferenceId)
        {
            if (_services.TryRemove(conferenceId, out var service))
                return service.DisposeAsync();

            return new ValueTask();
        }

        async ValueTask<IConferenceService> IConferenceServiceManager.GetService(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            return await GetService(conferenceId, services);
        }

        public async ValueTask<TService> GetService(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            if (!_services.TryGetValue(conferenceId, out var service))
            {
                service = await ServiceFactory(conferenceId, services);
                var actualService = _services.GetOrAdd(conferenceId, service);

                if (ReferenceEquals(service, actualService))
                    await service.InitializeAsync();
                else service.DisposeAsync().Forget();

                return actualService;
            }

            return service;
        }

        protected abstract ValueTask<TService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services);
    }
}