using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Infrastructure.Conferencing;
using PaderConference.Infrastructure.Extensions;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionsServiceManager : ConferenceServiceManager<PermissionsService>
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly ILogger<PermissionsService> _logger;
        private readonly IOptions<DefaultPermissionOptions> _options;
        private readonly IRedisDatabase _redisDatabase;

        public PermissionsServiceManager(IConferenceRepo conferenceRepo, IRedisDatabase redisDatabase,
            IOptions<DefaultPermissionOptions> options, ILogger<PermissionsService> logger)
        {
            _conferenceRepo = conferenceRepo;
            _redisDatabase = redisDatabase;
            _options = options;
            _logger = logger;
        }

        protected override ValueTask<PermissionsService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            return new PermissionsService(conferenceId, _conferenceRepo, _redisDatabase, _options, _logger)
                .ToValueTask();
        }
    }
}