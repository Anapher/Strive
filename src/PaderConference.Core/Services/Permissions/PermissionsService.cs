using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Permissions.Messages;
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
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IPermissionsRepo _permissionsRepo;
        private DefaultPermissionOptions _defaultPermissions;

        private readonly ConcurrentBag<FetchPermissionsDelegate> _fetchPermissionsDelegates =
            new ConcurrentBag<FetchPermissionsDelegate>();

        private readonly ILogger<PermissionsService> _logger;
        private readonly ISignalMessenger _clients;
        private readonly IConnectionMapping _connectionMapping;
        private readonly IConferenceManager _conferenceManager;

        private readonly DatabasePermissionValues _databasePermissionValues;

        private IImmutableDictionary<string, IImmutableDictionary<string, JsonElement>> _temporaryPermissions =
            ImmutableDictionary<string, IImmutableDictionary<string, JsonElement>>.Empty;

        private readonly AsyncReaderWriterLock _permissionLock = new AsyncReaderWriterLock();

        private readonly IDisposable _optionsDisposable;

        public PermissionsService(string conferenceId, IConferenceRepo conferenceRepo, IPermissionsRepo permissionsRepo,
            ISignalMessenger clients, IConnectionMapping connectionMapping, IConferenceManager conferenceManager,
            IOptionsMonitor<DefaultPermissionOptions> defaultPermissions, ILogger<PermissionsService> logger)
        {
            _conferenceId = conferenceId;
            _conferenceRepo = conferenceRepo;
            _permissionsRepo = permissionsRepo;
            _clients = clients;
            _connectionMapping = connectionMapping;
            _conferenceManager = conferenceManager;
            _defaultPermissions = defaultPermissions.CurrentValue;
            _logger = logger;

            _optionsDisposable = defaultPermissions.OnChange(OnDefaultPermissionChanged);

            _databasePermissionValues =
                new DatabasePermissionValues(conferenceRepo, conferenceManager, conferenceId, RefreshPermissions);

            RegisterLayerProvider(FetchPermissions);
        }

        public override async ValueTask InitializeAsync()
        {
            await _databasePermissionValues.InitializeAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            _optionsDisposable.Dispose();
            await _databasePermissionValues.DisposeAsync();
        }

        public override ValueTask InitializeParticipant(Participant participant)
        {
            return RefreshPermissions(new[] {participant});
        }

        public void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions)
        {
            _fetchPermissionsDelegates.Add(fetchPermissions);
        }

        public async ValueTask SetTemporaryPermission(IServiceMessage<SetTemporaryPermissionMessage> message)
        {
            var permissions = await GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Conference.CanGiveTemporaryPermission))
            {
                await message.ResponseError(PermissionsError.PermissionDeniedGiveTemporaryPermission);
                return;
            }

            if (!PermissionsListUtil.All.TryGetValue(message.Payload.PermissionKey, out var descriptor))
            {
                await message.ResponseError(PermissionsError.PermissionKeyNotFound);
                return;
            }

            var participant = _conferenceManager.GetParticipants(_conferenceId)
                .FirstOrDefault(x => x.ParticipantId == message.Payload.ParticipantId);
            if (participant == null)
            {
                await message.ResponseError(PermissionsError.ParticipantNotFound);
                return;
            }

            if (message.Payload.Value != null)
            {
                if (!descriptor.ValidateValue(message.Payload.Value.Value))
                {
                    await message.ResponseError(PermissionsError.InvalidPermissionValueType);
                    return;
                }

                using (await _permissionLock.WriterLockAsync())
                {
                    if (!_temporaryPermissions.TryGetValue(message.Payload.ParticipantId, out var newPermissions))
                        newPermissions = ImmutableDictionary<string, JsonElement>.Empty;

                    newPermissions = newPermissions.SetItem(descriptor.Key, message.Payload.Value.Value);

                    _temporaryPermissions =
                        _temporaryPermissions.SetItem(message.Payload.ParticipantId, newPermissions);
                }
            }
            else
            {
                using (await _permissionLock.WriterLockAsync())
                {
                    if (!_temporaryPermissions.TryGetValue(message.Payload.ParticipantId, out var newPermissions))
                        return;

                    newPermissions = newPermissions.Remove(descriptor.Key);

                    _temporaryPermissions =
                        _temporaryPermissions.SetItem(message.Payload.ParticipantId, newPermissions);
                }
            }

            await RefreshPermissions(new[] {participant});
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
                new PermissionLayer(PermissionLayer.PERMISSION_LAYER_CONFERENCE_DEFAULT,
                    _defaultPermissions.Conference),
                new PermissionLayer(PermissionLayer.PERMISSION_LAYER_CONFERENCE,
                    _databasePermissionValues.ConferencePermissions ??
                    ImmutableDictionary<string, JsonElement>.Empty),
            };

            if (_databasePermissionValues.Moderators.Contains(participant.ParticipantId))
                result.AddRange(new[]
                {
                    new PermissionLayer(PermissionLayer.PERMISSION_LAYER_MODERATOR_DEFAULT,
                        _defaultPermissions.Moderator),
                    new PermissionLayer(PermissionLayer.PERMISSION_LAYER_MODERATOR,
                        _databasePermissionValues.ModeratorPermissions ??
                        ImmutableDictionary<string, JsonElement>.Empty),
                });

            if (_temporaryPermissions.TryGetValue(participant.ParticipantId, out var temporaryPermissions))
                result.Add(new PermissionLayer(PermissionLayer.PERMISSION_LAYER_TEMPORARY, temporaryPermissions));

            return new ValueTask<IEnumerable<PermissionLayer>>(result);
        }

        private async void OnDefaultPermissionChanged(DefaultPermissionOptions obj)
        {
            _logger.LogInformation("Default permissions updated. Update conference permissions.");
            _defaultPermissions = obj;

            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference != null)
                await _databasePermissionValues.OnConferenceUpdated(conference);
        }
    }
}
