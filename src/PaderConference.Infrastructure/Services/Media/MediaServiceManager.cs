using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Hubs;
using PaderConference.Infrastructure.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Media
{
    public class MediaServiceManager : ConferenceServiceManager<MediaService>
    {
        private readonly IRtcMediaConnectionFactory _factory;
        private readonly IHubContext<CoreHub> _hubContext;

        public MediaServiceManager(IRtcMediaConnectionFactory factory, IHubContext<CoreHub> hubContext)
        {
            _factory = factory;
            _hubContext = hubContext;
        }

        protected override MediaService ServiceFactory(Conference conference,
            IEnumerable<IConferenceServiceManager> services)
        {
            var synchronizeService = services.OfType<IConferenceServiceManager<SynchronizationService>>().First();
            return new MediaService(conference, _hubContext.Clients, _factory,
                synchronizeService.GetService(conference, services));
        }
    }
}