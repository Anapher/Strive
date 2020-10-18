using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Conferencing;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Hubs;
using PaderConference.Infrastructure.Sockets;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionsServiceManager : ConferenceServiceManager<PermissionsService>
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly ILogger<PermissionsService> _logger;
        private readonly IOptionsMonitor<DefaultPermissionOptions> _options;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly IConnectionMapping _connectionMapping;
        private readonly IConferenceManager _conferenceManager;

        public PermissionsServiceManager(IConferenceRepo conferenceRepo, IRedisDatabase redisDatabase,
            IHubContext<CoreHub> hubContext, IConnectionMapping connectionMapping, IConferenceManager conferenceManager,
            IOptionsMonitor<DefaultPermissionOptions> options, ILogger<PermissionsService> logger)
        {
            _conferenceRepo = conferenceRepo;
            _redisDatabase = redisDatabase;
            _hubContext = hubContext;
            _connectionMapping = connectionMapping;
            _conferenceManager = conferenceManager;
            _options = options;
            _logger = logger;
        }

        protected override ValueTask<PermissionsService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            return new PermissionsService(conferenceId, _conferenceRepo, _redisDatabase, _hubContext.Clients,
                _connectionMapping, _conferenceManager, _options, _logger).ToValueTask();
        }
    }
}