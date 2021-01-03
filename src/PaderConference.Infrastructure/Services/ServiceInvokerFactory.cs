using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;

namespace PaderConference.Infrastructure.Services
{
    public class ServiceInvokerFactory : IServiceInvokerFactory
    {
        private readonly IConnectionMapping _connectionMapping;
        private readonly IComponentContext _componentContext;
        private readonly IConferenceManager _conferenceManager;
        private readonly IEnumerable<IConferenceServiceManager> _conferenceServices;
        private readonly ILogger<ServiceInvoker> _logger;

        public ServiceInvokerFactory(IConnectionMapping connectionMapping, IComponentContext componentContext,
            IConferenceManager conferenceManager, IEnumerable<IConferenceServiceManager> conferenceServices,
            ILogger<ServiceInvoker> logger)
        {
            _connectionMapping = connectionMapping;
            _componentContext = componentContext;
            _conferenceManager = conferenceManager;
            _conferenceServices = conferenceServices;
            _logger = logger;
        }

        public IServiceInvoker CreateForHub(Hub hub)
        {
            var context = new HubServiceInvokerContext(hub);
            return new ServiceInvoker(_connectionMapping, _componentContext, _conferenceManager, _conferenceServices,
                context, _logger);
        }
    }
}
