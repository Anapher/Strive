using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services;

namespace PaderConference.Infrastructure.ServiceFactories.Base
{
    public class AutowiredConferenceServiceManager<TService> : ConferenceServiceManager<TService>
        where TService : ConferenceService
    {
        private readonly IComponentContext _context;

        public AutowiredConferenceServiceManager(IComponentContext context)
        {
            _context = context;
        }

        protected override async ValueTask<TService> ServiceFactory(string conferenceId)
        {
            var disposables = new List<IAsyncDisposable>();

            var parameters = await GetParameters(conferenceId, disposables).ToListAsync();
            parameters.Add(new NamedParameter("conferenceId", conferenceId));

            TService service;
            try
            {
                service = _context.Resolve<TService>(parameters.ToArray());
            }
            catch (Exception)
            {
                foreach (var disposable in disposables)
                {
                    await disposable.DisposeAsync();
                }

                throw;
            }

            foreach (var disposable in disposables)
            {
                service.RegisterDisposable(disposable);
            }

            return service;
        }

        protected virtual IAsyncEnumerable<Parameter> GetParameters(string conferenceId,
            IList<IAsyncDisposable> disposables)
        {
            return AsyncEnumerable.Empty<Parameter>();
        }

        protected async ValueTask<Parameter> ResolveServiceAsync<T>(string conferenceId) where T : IConferenceService
        {
            var serviceManager = _context.Resolve<IConferenceServiceManager<T>>();
            var service = await serviceManager.GetService(conferenceId);
            return new TypedParameter(typeof(T), service);
        }

        protected async ValueTask<Parameter> ResolveOptions<T>(string conferenceId, IList<IAsyncDisposable> disposables,
            Func<Conference, T> selector) where T : class, new()
        {
            var conferenceRepo = _context.Resolve<IConferenceRepo>();
            var options = new UseConferenceSelector<T>(conferenceId, conferenceRepo, selector, new T());
            await options.InitializeAsync();
            disposables.Add(options);

            return new TypedParameter(typeof(T), options);
        }
    }
}
