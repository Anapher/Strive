using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.ConferenceControl
{
    public class ConferenceControlService : ConferenceService
    {
        private readonly string _conferenceId;
        private readonly IConferenceManager _conferenceManager;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<ConferenceControlService> _logger;
        private readonly ISignalMessenger _messenger;
        private readonly IOptions<ConferenceSchedulerOptions> _options;
        private readonly IPermissionsService _permissionsService;
        private readonly ISynchronizedObject<SynchronizedConferenceInfo> _synchronizedObject;

        public ConferenceControlService(Conference conference, IConferenceManager conferenceManager,
            IConferenceRepo conferenceRepo, ISynchronizationManager synchronizationManager,
            IPermissionsService permissionsService, ISignalMessenger messenger, IConnectionMapping connectionMapping,
            IOptions<ConferenceSchedulerOptions> options, ILogger<ConferenceControlService> logger)
        {
            _conferenceId = conference.ConferenceId;

            _conferenceManager = conferenceManager;
            _conferenceRepo = conferenceRepo;
            _permissionsService = permissionsService;
            _messenger = messenger;
            _connectionMapping = connectionMapping;
            _options = options;
            _logger = logger;

            _synchronizedObject = synchronizationManager.Register("conferenceState",
                new SynchronizedConferenceInfo(conference));
        }

        public override async ValueTask InitializeAsync()
        {
            using var _ = _logger.BeginMethodScope(new Dictionary<string, object> {{"conferenceId", _conferenceId}});

            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference != null) await OnConferenceUpdated(conference);
            else _logger.LogError("The conference was not found in database.");

            RegisterDisposable(await _conferenceRepo.SubscribeConferenceUpdated(_conferenceId, OnConferenceUpdated));

            _conferenceManager.ConferenceOpened += ConferenceManagerOnConferenceOpened;
            _conferenceManager.ConferenceClosed += ConferenceManagerOnConferenceClosed;
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
            await base.DisposeAsync();

            _conferenceManager.ConferenceOpened -= ConferenceManagerOnConferenceOpened;
            _conferenceManager.ConferenceClosed -= ConferenceManagerOnConferenceClosed;
        }

        public async ValueTask<SuccessOrError> OpenConference(IServiceMessage message)
        {
            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Conference.CanOpenAndClose))
                return CommonError.PermissionDenied(PermissionsList.Conference.CanOpenAndClose);

            await _conferenceManager.OpenConference(_conferenceId);
            return SuccessOrError.Succeeded;
        }

        public async ValueTask<SuccessOrError> CloseConference(IServiceMessage message)
        {
            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Conference.CanOpenAndClose))
                return CommonError.PermissionDenied(PermissionsList.Conference.CanOpenAndClose);

            await _conferenceManager.CloseConference(_conferenceId);
            return SuccessOrError.Succeeded;
        }

        public async ValueTask<SuccessOrError> KickParticipant(IServiceMessage<KickParticipantRequest> message)
        {
            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Conference.CanKickParticipant))
                return CommonError.PermissionDenied(PermissionsList.Conference.CanKickParticipant);

            if (!_connectionMapping.ConnectionsR.TryGetValue(message.Payload.ParticipantId, out var connections))
                return ConferenceError.ParticipantConnectionNotFound;

            await _messenger.SendToConnectionAsync(connections.MainConnectionId,
                CoreHubMessages.Response.OnRequestDisconnect, null);
            return SuccessOrError.Succeeded;
        }

        private async Task OnConferenceUpdated(Conference arg)
        {
            var scheduledDate = GetNextExecution(arg.Configuration);
            var open = await _conferenceManager.GetIsConferenceOpen(arg.ConferenceId);

            await _synchronizedObject.Update(
                new SynchronizedConferenceInfo(arg) {ScheduledDate = scheduledDate, IsOpen = open});
        }

        private DateTimeOffset? GetNextExecution(ConferenceConfiguration config)
        {
            var now = DateTimeOffset.UtcNow;

            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.Value.CronTimezone);
            }
            catch (Exception e)
            {
                timeZone = TimeZoneInfo.Local;
                _logger.LogWarning(e,
                    "Error occurred on finding the time zone \"{timeZone}\". Please make sure to select a valid time zone. Local time zone ({local}) is used.",
                    _options.Value.CronTimezone, timeZone);
            }

            return ConferenceScheduler.GetNextExecution(config, now, timeZone);
        }
    }
}
