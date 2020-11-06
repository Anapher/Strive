using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services;

namespace PaderConference.Infrastructure.ServiceFactories.Base
{
    public class AutowiredConferenceServiceManager<TService> : ConferenceServiceManager<TService>
        where TService : IConferenceService
    {
        private readonly IComponentContext _context;
        private readonly Lazy<IReadOnlyDictionary<Type, IConferenceServiceManager>> _typeToServiceManager;

        public AutowiredConferenceServiceManager(IComponentContext context)
        {
            _context = context;

            _typeToServiceManager = new Lazy<IReadOnlyDictionary<Type, IConferenceServiceManager>>(
                () => context.Resolve<IEnumerable<IConferenceServiceManager>>()
                    .SelectMany(serviceManager =>
                        serviceManager.ServiceType.GetInterfaces().Select(x => (x, serviceManager))).GroupBy(x => x.x)
                    .ToDictionary(x => x.Key, x => x.First().serviceManager),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        protected override ValueTask<TService> ServiceFactory(string conferenceId)
        {
            var serviceParam = new ResolvedParameter(
                (param, context) => _typeToServiceManager.Value.ContainsKey(param.ParameterType), (param, context) =>
                {
                    var manager = _typeToServiceManager.Value[param.ParameterType];
                    return manager.GetService(conferenceId).Result; // TODO: maybe execute this task async before?
                });

            return _context.Resolve<TService>(new NamedParameter("conferenceId", conferenceId), serviceParam)
                .ToValueTask();
        }
    }
}
