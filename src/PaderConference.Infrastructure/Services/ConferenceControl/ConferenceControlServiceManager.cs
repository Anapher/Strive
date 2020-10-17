using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.ConferenceControl
{
    public class ConferenceControlServiceManager : ConferenceServiceManager<ConferenceControlService>
    {
        private readonly IConferenceManager _conferenceManager;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IConferenceScheduler _conferenceScheduler;
        private readonly ILogger<ConferenceControlService> _logger;
        private readonly IRedisDatabase _redisDatabase;

        public ConferenceControlServiceManager(IConferenceScheduler conferenceScheduler,
            IConferenceManager conferenceManager, IConferenceRepo conferenceRepo, IRedisDatabase redisDatabase,
            ILogger<ConferenceControlService> logger)
        {
            _conferenceScheduler = conferenceScheduler;
            _conferenceManager = conferenceManager;
            _conferenceRepo = conferenceRepo;
            _redisDatabase = redisDatabase;
            _logger = logger;
        }

        protected override async ValueTask<ConferenceControlService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            var synchronizeService = await services.OfType<IConferenceServiceManager<SynchronizationService>>().First()
                .GetService(conferenceId, services);
            var permissionsService = await services.OfType<IConferenceServiceManager<PermissionsService>>().First()
                .GetService(conferenceId, services);

            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null) throw new InvalidOperationException("Conference not found.");

            return new ConferenceControlService(conference, _conferenceScheduler, _conferenceManager, _conferenceRepo,
                synchronizeService, permissionsService, _redisDatabase, _logger);
        }
    }
}