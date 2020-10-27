using System;
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
        private readonly IConferenceServiceManager<SynchronizationService> _synchronizationServiceManager;
        private readonly IConferenceServiceManager<PermissionsService> _permissionsServiceManager;

        public ConferenceControlServiceManager(IConferenceScheduler conferenceScheduler,
            IConferenceManager conferenceManager, IConferenceRepo conferenceRepo, IRedisDatabase redisDatabase,
            IConferenceServiceManager<SynchronizationService> synchronizationServiceManager,
            IConferenceServiceManager<PermissionsService> permissionsServiceManager,
            ILogger<ConferenceControlService> logger)
        {
            _conferenceScheduler = conferenceScheduler;
            _conferenceManager = conferenceManager;
            _conferenceRepo = conferenceRepo;
            _redisDatabase = redisDatabase;
            _synchronizationServiceManager = synchronizationServiceManager;
            _permissionsServiceManager = permissionsServiceManager;
            _logger = logger;
        }

        protected override async ValueTask<ConferenceControlService> ServiceFactory(string conferenceId)
        {
            var synchronizeService = await _synchronizationServiceManager.GetService(conferenceId);
            var permissionsService = await _permissionsServiceManager.GetService(conferenceId);

            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null) throw new InvalidOperationException("Conference not found.");

            return new ConferenceControlService(conference, _conferenceScheduler, _conferenceManager, _conferenceRepo,
                synchronizeService, permissionsService, _redisDatabase, _logger);
        }
    }
}