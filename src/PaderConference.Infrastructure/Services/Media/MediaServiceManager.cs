using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Infrastructure.Hubs;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Media
{
    public class MediaServiceManager : ConferenceServiceManager<MediaService>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly IRedisDatabase _redisDatabase;

        public MediaServiceManager(IHubContext<CoreHub> hubContext, IRedisDatabase redisDatabase)
        {
            _hubContext = hubContext;
            _redisDatabase = redisDatabase;
        }

        protected override async ValueTask<MediaService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            var synchronizeService = await services.OfType<IConferenceServiceManager<SynchronizationService>>().First()
                .GetService(conferenceId, services);
            var permissionsService = await services.OfType<IConferenceServiceManager<PermissionsService>>().First()
                .GetService(conferenceId, services);

            return new MediaService(conferenceId, _hubContext.Clients, synchronizeService, _redisDatabase,
                permissionsService);
        }
    }
}