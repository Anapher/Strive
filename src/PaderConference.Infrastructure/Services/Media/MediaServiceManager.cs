using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Infrastructure.Hubs;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;
using PaderConference.Infrastructure.Sockets;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Media
{
    public class MediaServiceManager : ConferenceServiceManager<MediaService>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly ILogger<MediaService> _logger;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IConnectionMapping _connectionMapping;

        public MediaServiceManager(IHubContext<CoreHub> hubContext, IRedisDatabase redisDatabase,
            IConnectionMapping connectionMapping, ILogger<MediaService> logger)
        {
            _hubContext = hubContext;
            _redisDatabase = redisDatabase;
            _connectionMapping = connectionMapping;
            _logger = logger;
        }

        protected override async ValueTask<MediaService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            var synchronizeService = await services.OfType<IConferenceServiceManager<SynchronizationService>>().First()
                .GetService(conferenceId, services);
            var permissionsService = await services.OfType<IConferenceServiceManager<PermissionsService>>().First()
                .GetService(conferenceId, services);

            return new MediaService(conferenceId, _hubContext.Clients, synchronizeService, _redisDatabase,
                permissionsService, _connectionMapping, _logger);
        }
    }
}