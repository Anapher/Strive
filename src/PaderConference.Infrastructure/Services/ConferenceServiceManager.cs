using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace PaderConference.Infrastructure.Services
{
    public abstract class ConferenceServiceManager<TService> : IConferenceServiceManager<TService>
        where TService : IConferenceService
    {
        private readonly AsyncLock _serviceCreationLock = new AsyncLock();

        private readonly ConcurrentDictionary<string, TService> _services =
            new ConcurrentDictionary<string, TService>();

        public ValueTask Close(string conferenceId)
        {
            if (_services.TryRemove(conferenceId, out var service))
                return service.DisposeAsync();

            return new ValueTask();
        }

        async ValueTask<IConferenceService> IConferenceServiceManager.GetService(string conferenceId)
        {
            return await GetService(conferenceId);
        }

        public async ValueTask<TService> GetService(string conferenceId)
        {
            if (!_services.TryGetValue(conferenceId, out var service))
            {
                // double check lock
                using (await _serviceCreationLock.LockAsync())
                {
                    if (!_services.TryGetValue(conferenceId, out service))
                    {
                        service = await ServiceFactory(conferenceId);
                        await service.InitializeAsync();

                        if (!_services.TryAdd(conferenceId, service)) throw new InvalidOperationException("wtf");
                    }
                }

                return service;
            }

            return service;
        }

        protected abstract ValueTask<TService> ServiceFactory(string conferenceId);
    }
}