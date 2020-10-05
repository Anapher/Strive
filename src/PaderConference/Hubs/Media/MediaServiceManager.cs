using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Hubs.Media
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

        protected override MediaService ServiceFactory(Conference conference)
        {
            return new MediaService(conference, _hubContext, _factory);
        }
    }
}