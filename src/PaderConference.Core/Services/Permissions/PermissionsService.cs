using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Errors;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Permissions.Dto;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     The permission service manages the permissions of each participant. The strategy is as follows: Other services may
    ///     register <see cref="FetchPermissionsDelegate" /> that return depending on the participant different permission
    ///     layers. These layers are then merged and saved to database so they are accessible for the SFU. If one wants to get
    ///     participant permissions, we just return <see cref="RepositoryPermissions" /> that call the repository each time
    ///     permissions are requested. If something changes (e. g. a participant is promoted, changes the room, etc.),
    ///     <see cref="RefreshPermissions" /> must be called to update the permissions of the participant in database.
    /// </summary>
    public class PermissionsService : ConferenceService, IPermissionsService
    {
        private readonly string _conferenceId;
        private readonly IPermissionsRepo _permissionsRepo;
        private DefaultPermissionOptions _defaultPermissions;

        private readonly ConcurrentBag<FetchPermissionsDelegate> _fetchPermissionsDelegates = new();

        private readonly ILogger<PermissionsService> _logger;
        private readonly ISignalMessenger _clients;
        private readonly IConnectionMapping _connectionMapping;
        private readonly IConferenceManager _conferenceManager;

        private readonly ConferenceConfigWatcher _conferenceConfigWatcher;
        private readonly IDisposable _optionsDisposable;

        // Participant Id -> (permission key -> permission value)
        private readonly ISynchronizedObject<IImmutableDictionary<string, IImmutableDictionary<string, JsonElement>>>
            _tempPermissions;

        public PermissionsService(string conferenceId, IConferenceRepo conferenceRepo, IPermissionsRepo permissionsRepo,
            ISignalMessenger clients, IConnectionMapping connectionMapping, IConferenceManager conferenceManager,
            ISynchronizationManager synchronizationManager,
            IOptionsMonitor<DefaultPermissionOptions> defaultPermissions, ILogger<PermissionsService> logger)
        {
            _conferenceId = conferenceId;
            _permissionsRepo = permissionsRepo;
            _clients = clients;
            _connectionMapping = connectionMapping;
            _conferenceManager = conferenceManager;
            _defaultPermissions = defaultPermissions.CurrentValue;
            _logger = logger;

            _optionsDisposable = defaultPermissions.OnChange(OnDefaultPermissionChanged);

            _conferenceConfigWatcher =
                new ConferenceConfigWatcher(conferenceId, conferenceRepo, conferenceManager, RefreshPermissions);

            _tempPermissions =
                synchronizationManager
                    .Register<IImmutableDictionary<string, IImmutableDictionary<string, JsonElement>>>(
                        "tempPermissions", ImmutableDictionary<string, IImmutableDictionary<string, JsonElement>>.Empty,
                        ParticipantGroup.Moderators);

            RegisterLayerProvider(FetchPermissions);
        }

        public override async ValueTask InitializeAsync()
        {
            await _conferenceConfigWatcher.InitializeAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            _optionsDisposable.Dispose();
            await _conferenceConfigWatcher.DisposeAsync();
        }

        public override ValueTask InitializeParticipant(Participant participant)
        {
            return RefreshPermissions(new[] {participant});
        }

        public void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions)
        {
            _fetchPermissionsDelegates.Add(fetchPermissions);
        }

        public async ValueTask<ISuccessOrError> SetTemporaryPermission(
            IServiceMessage<SetTemporaryPermissionDto> message)
        {
            using var _ = _logger.BeginMethodScope();
            var (targetParticipantId, permissionKey, value) = message.Payload;

            if (targetParticipantId == null)
                return SuccessOrError.Failed(new FieldValidationError(nameof(message.Payload.ParticipantId),
                    "You must provide a participant id."));

            if (permissionKey == null)
                return SuccessOrError.Failed(new FieldValidationError(nameof(message.Payload.PermissionKey),
                    "You must provide a permission key."));

            var permissions = await GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Permissions.CanGiveTemporaryPermission))
                return SuccessOrError.Failed(PermissionsError.PermissionDeniedGiveTemporaryPermission);

            _logger.LogDebug("Set temporary permission \"{permissionKey}\" of participant {participantId} to {value}",
                permissionKey, targetParticipantId, value);

            if (!PermissionsListUtil.All.TryGetValue(permissionKey, out var descriptor))
                return SuccessOrError.Failed(PermissionsError.PermissionKeyNotFound);

            var participant = _conferenceManager.GetParticipants(_conferenceId)
                .FirstOrDefault(x => x.ParticipantId == targetParticipantId);
            if (participant == null) return SuccessOrError.Failed(CommonError.ParticipantNotFound);

            if (value != null)
            {
                if (!descriptor.ValidateValue(value.Value))
                    return SuccessOrError.Failed(PermissionsError.InvalidPermissionValueType);

                await _tempPermissions.Update(current =>
                {
                    if (!current.TryGetValue(targetParticipantId, out var newPermissions))
                        newPermissions = ImmutableDictionary<string, JsonElement>.Empty;

                    newPermissions = newPermissions.SetItem(descriptor.Key, value.Value);
                    return current.SetItem(targetParticipantId, newPermissions);
                });
            }
            else
            {
                if (!_tempPermissions.Current.TryGetValue(targetParticipantId, out var _))
                    return SuccessOrError.Succeeded;

                await _tempPermissions.Update(current =>
                {
                    if (!current.TryGetValue(targetParticipantId, out var newPermissions))
                        return current;

                    newPermissions = newPermissions.Remove(descriptor.Key);
                    return current.SetItem(targetParticipantId, newPermissions);
                });
            }

            await RefreshPermissions(new[] {participant});
            return SuccessOrError.Succeeded;
        }

        public async ValueTask<SuccessOrError<ParticipantPermissionInfo>> FetchPermissions(
            IServiceMessage<string?> message)
        {
            using var _ = _logger.BeginMethodScope();
            var participantId = message.Payload ?? message.Participant.ParticipantId;

            Participant participant;

            // you may always fetch your own permissions
            if (participantId != message.Participant.ParticipantId)
            {
                var myPermissions = await GetPermissions(message.Participant);
                if (!await myPermissions.GetPermission(PermissionsList.Permissions.CanSeeAnyParticipantsPermissions))
                    return PermissionsError.PermissionDeniedGiveTemporaryPermission;

                if (!_conferenceManager.TryGetParticipant(_conferenceId, participantId, out participant!))
                    return CommonError.ParticipantNotFound;
            }
            else
            {
                participant = message.Participant;
            }

            var permissions = await FetchPermissions(participant);
            return new ParticipantPermissionInfo(participantId, permissions);
        }

        public ValueTask<IPermissionStack> GetPermissions(Participant participant)
        {
            return new RepositoryPermissions(_permissionsRepo, participant.ParticipantId)
                .ToValueTask<IPermissionStack>();
        }

        public async ValueTask RefreshPermissions(IEnumerable<Participant> participants)
        {
            var newPermissions = new List<(Participant, Dictionary<string, JsonElement>)>();
            foreach (var participant in participants)
            {
                newPermissions.Add((participant, await BuildFlattenPermissions(participant)));
            }

            _logger.LogDebug("Update permissions for {count} participants", newPermissions.Count);

            foreach (var (participant, permissions) in newPermissions)
            {
                await _permissionsRepo.SetPermissions(participant.ParticipantId, permissions);
            }

            await _permissionsRepo.PublishPermissionsUpdated(
                newPermissions.Select(x => x.Item1.ParticipantId).ToArray());

            foreach (var (participant, permissions) in newPermissions)
            {
                if (_connectionMapping.ConnectionsR.TryGetValue(participant.ParticipantId, out var connections))
                    await _clients.SendToConnectionAsync(connections.MainConnectionId,
                        CoreHubMessages.Response.OnPermissionsUpdated, permissions);
            }
        }

        private async ValueTask<Dictionary<string, JsonElement>> BuildFlattenPermissions(Participant participant)
        {
            var layers = new List<PermissionLayer>();
            foreach (var fetchPermissionsDelegate in _fetchPermissionsDelegates)
                layers.AddRange(await fetchPermissionsDelegate(participant));

            var stack = new PermissionStack(layers.OrderBy(x => x.Order).Select(x => x.Permissions).ToList());
            return stack.Flatten();
        }

        /// <summary>
        ///     Fetch default permissions defined in options
        /// </summary>
        private ValueTask<IEnumerable<PermissionLayer>> FetchPermissions(Participant participant)
        {
            var result = new List<PermissionLayer>
            {
                CommonPermissionLayers.ConferenceDefault(_defaultPermissions.Conference),
                CommonPermissionLayers.Conference(_conferenceConfigWatcher.ConferencePermissions ??
                                                  ImmutableDictionary<string, JsonElement>.Empty),
            };

            if (_conferenceConfigWatcher.Moderators.Contains(participant.ParticipantId))
                result.AddRange(new[]
                {
                    CommonPermissionLayers.ModeratorDefault(_defaultPermissions.Moderator),
                    CommonPermissionLayers.Moderator(_conferenceConfigWatcher.ModeratorPermissions ??
                                                     ImmutableDictionary<string, JsonElement>.Empty),
                });

            if (_tempPermissions.Current.TryGetValue(participant.ParticipantId, out var temporaryPermissions))
                result.Add(CommonPermissionLayers.Temporary(temporaryPermissions));

            return new ValueTask<IEnumerable<PermissionLayer>>(result);
        }

        private async void OnDefaultPermissionChanged(DefaultPermissionOptions obj)
        {
            _logger.LogInformation("Default permissions updated. Update conference permissions.");
            _defaultPermissions = obj;

            await _conferenceConfigWatcher.TriggerUpdate();
        }
    }
}
