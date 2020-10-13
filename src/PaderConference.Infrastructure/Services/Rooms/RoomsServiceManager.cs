using System.Collections.Generic;
using System.Linq;
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

        public RoomsServiceManager(IRedisDatabase redisDatabase, IOptions<RoomOptions> options,
            ILogger<RoomsService> logger)
        {
            _redisDatabase = redisDatabase;
            _options = options;
            _logger = logger;
        }

        protected override async ValueTask<RoomsService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            var permissionsService = await services.OfType<IConferenceServiceManager<PermissionsService>>().First()
                .GetService(conferenceId, services);
            var synchronizeService = await services.OfType<IConferenceServiceManager<SynchronizationService>>().First()
                .GetService(conferenceId, services);

            return new RoomsService(conferenceId, _redisDatabase, synchronizeService, permissionsService, _options,
                _logger);
        }
    }
}