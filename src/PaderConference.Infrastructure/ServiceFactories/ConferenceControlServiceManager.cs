using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Signaling;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class ConferenceControlServiceManager : ConferenceServiceManager<ConferenceControlService>
    {
        private readonly IConferenceManager _conferenceManager;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IConferenceScheduler _conferenceScheduler;
        private readonly ILogger<ConferenceControlService> _logger;
        private readonly IConferenceServiceManager<SynchronizationService> _synchronizationServiceManager;
        private readonly IConferenceServiceManager<PermissionsService> _permissionsServiceManager;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ISignalMessenger _signalMessenger;

        public ConferenceControlServiceManager(IConferenceScheduler conferenceScheduler,
            IConferenceManager conferenceManager, IConferenceRepo conferenceRepo,
            IConferenceServiceManager<SynchronizationService> synchronizationServiceManager,
            IConferenceServiceManager<PermissionsService> permissionsServiceManager,
            IConnectionMapping connectionMapping, ISignalMessenger signalMessenger,
            ILogger<ConferenceControlService> logger)
        {
            _conferenceScheduler = conferenceScheduler;
            _conferenceManager = conferenceManager;
            _conferenceRepo = conferenceRepo;
            _synchronizationServiceManager = synchronizationServiceManager;
            _permissionsServiceManager = permissionsServiceManager;
            _connectionMapping = connectionMapping;
            _signalMessenger = signalMessenger;
            _logger = logger;
        }

        protected override async ValueTask<ConferenceControlService> ServiceFactory(string conferenceId)
        {
            var synchronizeService = await _synchronizationServiceManager.GetService(conferenceId);
            var permissionsService = await _permissionsServiceManager.GetService(conferenceId);

            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null) throw new InvalidOperationException("Conference not found.");

            return new ConferenceControlService(conference, _conferenceScheduler, _conferenceManager, _conferenceRepo,
                synchronizeService, permissionsService, _signalMessenger, _connectionMapping, _logger);
        }
    }
}