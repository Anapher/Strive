using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Infrastructure.Extensions;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionsServiceManager : ConferenceServiceManager<PermissionsService>
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly ILogger<PermissionsService> _logger;
        private readonly IRedisDatabase _redisDatabase;

        public PermissionsServiceManager(IConferenceRepo conferenceRepo, IRedisDatabase redisDatabase,
            ILogger<PermissionsService> logger)
        {
            _conferenceRepo = conferenceRepo;
            _redisDatabase = redisDatabase;
            _logger = logger;
        }

        protected override ValueTask<PermissionsService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            return new PermissionsService(conferenceId, _conferenceRepo, _redisDatabase, _logger).ToValueTask();
        }
    }
}