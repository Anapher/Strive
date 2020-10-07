using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Hubs;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure.Services.Synchronization
{
    public class SynchronizationServiceManager : ConferenceServiceManager<SynchronizationService>
    {
        private readonly IConnectionMapping _connectionMapping;
        private readonly IHubContext<CoreHub> _hubContext;

        public SynchronizationServiceManager(IHubContext<CoreHub> hubContext, IConnectionMapping connectionMapping)
        {
            _hubContext = hubContext;
            _connectionMapping = connectionMapping;
        }

        protected override SynchronizationService ServiceFactory(Conference conference,
            IEnumerable<IConferenceServiceManager> services)
        {
            return new SynchronizationService(_hubContext.Clients, conference, _connectionMapping);
        }
    }
}