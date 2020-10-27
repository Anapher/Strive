using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Infrastructure.Hubs;
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
        private readonly IConferenceServiceManager<SynchronizationService> _synchronizationServiceManager;

        public MediaServiceManager(IHubContext<CoreHub> hubContext, IRedisDatabase redisDatabase,
            IConnectionMapping connectionMapping,
            IConferenceServiceManager<SynchronizationService> synchronizationServiceManager,
            ILogger<MediaService> logger)
        {
            _hubContext = hubContext;
            _redisDatabase = redisDatabase;
            _connectionMapping = connectionMapping;
            _synchronizationServiceManager = synchronizationServiceManager;
            _logger = logger;
        }

        protected override async ValueTask<MediaService> ServiceFactory(string conferenceId)
        {
            var synchronizeService = await _synchronizationServiceManager.GetService(conferenceId);

            return new MediaService(conferenceId, _hubContext.Clients, synchronizeService, _redisDatabase,
                _connectionMapping, _logger);
        }
    }
}