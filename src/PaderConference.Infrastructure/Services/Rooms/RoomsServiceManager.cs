using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Rooms
{
    public class RoomsServiceManager : ConferenceServiceManager<RoomsService>
    {
        private readonly ILogger<RoomsService> _logger;
        private readonly IOptions<RoomOptions> _options;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IConferenceServiceManager<PermissionsService> _permissionServiceManager;
        private readonly IConferenceServiceManager<SynchronizationService> _synchronizationServiceManager;

        public RoomsServiceManager(IRedisDatabase redisDatabase,
            IConferenceServiceManager<PermissionsService> permissionServiceManager,
            IConferenceServiceManager<SynchronizationService> synchronizationServiceManager,
            IOptions<RoomOptions> options, ILogger<RoomsService> logger)
        {
            _redisDatabase = redisDatabase;
            _permissionServiceManager = permissionServiceManager;
            _synchronizationServiceManager = synchronizationServiceManager;
            _options = options;
            _logger = logger;
        }

        protected override async ValueTask<RoomsService> ServiceFactory(string conferenceId)
        {
            var permissionsService = await _permissionServiceManager.GetService(conferenceId);
            var synchronizeService = await _synchronizationServiceManager.GetService(conferenceId);

            return new RoomsService(conferenceId, _redisDatabase, synchronizeService, permissionsService, _options,
                _logger);
        }
    }
}