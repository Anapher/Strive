using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Hubs;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.ConferenceControl
{
    public class ConferenceControlService : ConferenceService
    {
        private readonly string _conferenceId;
        private readonly IConferenceManager _conferenceManager;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IConferenceScheduler _conferenceScheduler;
        private readonly ILogger<ConferenceControlService> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IRedisDatabase _redisDatabase;
        private readonly ISynchronizedObject<SynchronizedConferenceInfo> _synchronizedObject;

        public ConferenceControlService(Conference conference, IConferenceScheduler conferenceScheduler,
            IConferenceManager conferenceManager, IConferenceRepo conferenceRepo,
            ISynchronizationManager synchronizationManager, IPermissionsService permissionsService,
            IRedisDatabase redisDatabase,
            ILogger<ConferenceControlService> logger)
        {
            _conferenceId = conference.ConferenceId;

            _conferenceScheduler = conferenceScheduler;
            _conferenceManager = conferenceManager;
            _conferenceRepo = conferenceRepo;
            _permissionsService = permissionsService;
            _redisDatabase = redisDatabase;
            _logger = logger;
            _synchronizedObject = synchronizationManager.Register("conferenceState",
                new SynchronizedConferenceInfo(conference, null, false));
        }

        public override async ValueTask InitializeAsync()
        {
            using (_logger.BeginScope("InitializeAsync()"))
            using (_logger.BeginScope(new Dictionary<string, object> {{"conferenceId", _conferenceId}}))
            {
                var conference = await _conferenceRepo.FindById(_conferenceId);
                if (conference != null) await OnConferenceUpdated(conference);
                else _logger.LogError("The conference was not found in database.");

                await _redisDatabase.SubscribeAsync<Conference>(RedisChannels.OnConferenceUpdated(_conferenceId),
                    OnConferenceUpdated);

                _conferenceManager.ConferenceOpened += ConferenceManagerOnConferenceOpened;
                _conferenceManager.ConferenceClosed += ConferenceManagerOnConferenceClosed;
            }
        }

        private async void ConferenceManagerOnConferenceClosed(object? sender, string e)
        {
            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference != null) await OnConferenceUpdated(conference);
            else _logger.LogError("The conference was not found in database.");
        }

        private void ConferenceManagerOnConferenceOpened(object? sender, Conference e)
        {
            if (e.ConferenceId == _conferenceId) OnConferenceUpdated(e).Forget();
        }

        public override async ValueTask DisposeAsync()
        {
            await _redisDatabase.UnsubscribeAsync<Conference>(RedisChannels.OnConferenceUpdated(_conferenceId),
                OnConferenceUpdated);

            _conferenceManager.ConferenceOpened -= ConferenceManagerOnConferenceOpened;
            _conferenceManager.ConferenceClosed -= ConferenceManagerOnConferenceClosed;
        }

        public async ValueTask OpenConference(IServiceMessage message)
        {
            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Conference.CanOpenAndClose))
            {
                await message.ResponseError(ConferenceError.PermissionDeniedToOpenOrClose);
                return;
            }

            await _conferenceManager.OpenConference(_conferenceId);
        }

        public async ValueTask CloseConference(IServiceMessage message)
        {
            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Conference.CanOpenAndClose))
            {
                await message.ResponseError(ConferenceError.PermissionDeniedToOpenOrClose);
                return;
            }

            await _conferenceManager.CloseConference(_conferenceId);
        }

        private async Task OnConferenceUpdated(Conference arg)
        {
            var scheduledDate = await _conferenceScheduler.GetNextOccurrence(arg.ConferenceId);
            var open = await _conferenceManager.GetIsConferenceOpen(arg.ConferenceId);

            await _synchronizedObject.Update(new SynchronizedConferenceInfo(arg, scheduledDate, open));
        }
    }
}