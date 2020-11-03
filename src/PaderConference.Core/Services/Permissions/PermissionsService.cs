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

        private IImmutableDictionary<string, JsonElement>? _conferencePermissions;
        private IImmutableDictionary<string, JsonElement>? _moderatorPermissions;
        private IImmutableList<string> _moderators;

        private IImmutableDictionary<string, IImmutableDictionary<string, JsonElement>> _temporaryPermissions =
            ImmutableDictionary<string, IImmutableDictionary<string, JsonElement>>.Empty;

        private readonly AsyncReaderWriterLock _permissionLock = new AsyncReaderWriterLock();

        private readonly IDisposable _optionsDisposable;
        private Func<Task>? _unsubscribeConferenceUpdated;

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

            _moderators = ImmutableList<string>.Empty;

            RegisterLayerProvider(FetchPermissions);
        }

        public override async ValueTask InitializeAsync()
        {
            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference == null)
            {
                _logger.LogError("Conference was not found in database.");
                return;
            }

            _moderators = conference.Moderators;
            _conferencePermissions = ParseDictionary(conference.Permissions);
            _moderatorPermissions = ParseDictionary(conference.ModeratorPermissions);

            _unsubscribeConferenceUpdated =
                await _conferenceRepo.SubscribeConferenceUpdated(_conferenceId, OnConferenceUpdated);
        }

        public override async ValueTask DisposeAsync()
        {
            _optionsDisposable.Dispose();

            if (_unsubscribeConferenceUpdated != null)
            {
                await _unsubscribeConferenceUpdated();
                _unsubscribeConferenceUpdated = null;
            }
        }

        public override ValueTask InitializeParticipant(Participant participant)
        {
            return UpdatePermissions(new[] {participant});
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
                ?.FirstOrDefault(x => x.ParticipantId == message.Payload.ParticipantId);
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

            await UpdatePermissions(new[] {participant});
        }

        public ValueTask<IPermissionStack> GetPermissions(Participant participant)
        {
            return new RedisPermissions(_permissionsRepo, participant.ParticipantId).ToValueTask<IPermissionStack>();
        }

        // ReSharper disable PossibleMultipleEnumeration
        public async ValueTask UpdatePermissions(IEnumerable<Participant> participants)
        {
            var newPermissions = new List<(Participant, Dictionary<string, JsonElement>)>();
            foreach (var participant in participants)
                newPermissions.Add((participant, await BuildFlattenPermissions(participant)));

            foreach (var participant in participants)
            {
                var permissions = await BuildFlattenPermissions(participant);
                await _permissionsRepo.SetPermissions(participant.ParticipantId, permissions);
            }

            await _permissionsRepo.PublishPermissionsUpdated(participants.Select(x => x.ParticipantId).ToArray());

            foreach (var (participant, permissions) in newPermissions)
                if (_connectionMapping.ConnectionsR.TryGetValue(participant.ParticipantId, out var connections))
                    await _clients.SendToConnectionAsync(connections.MainConnectionId,
                        CoreHubMessages.Response.OnPermissionsUpdated, permissions);
        }
        // ReSharper restore PossibleMultipleEnumeration

        private async ValueTask<Dictionary<string, JsonElement>> BuildFlattenPermissions(Participant participant)
        {
            var layers = new List<PermissionLayer>();
            foreach (var fetchPermissionsDelegate in _fetchPermissionsDelegates)
                layers.AddRange(await fetchPermissionsDelegate(participant));

            var stack = new PermissionStack(layers.OrderBy(x => x.Order).Select(x => x.Permissions).ToList());
            return stack.Flatten();
        }

        private ValueTask<IEnumerable<PermissionLayer>> FetchPermissions(Participant participant)
        {
            var result = new List<PermissionLayer>
            {
                new PermissionLayer(10, _conferencePermissions ?? _defaultPermissions.Conference),
            };

            if (_moderators.Contains(participant.ParticipantId))
                result.Add(new PermissionLayer(30, _moderatorPermissions ?? _defaultPermissions.Moderator));

            if (_temporaryPermissions.TryGetValue(participant.ParticipantId, out var temporaryPermissions))
                result.Add(new PermissionLayer(100, temporaryPermissions));

            return new ValueTask<IEnumerable<PermissionLayer>>(result);
        }

        private async Task OnConferenceUpdated(Conference arg)
        {
            var participants = _conferenceManager.GetParticipants(_conferenceId);
            if (participants == null) return; // conference doesn't exist

            var updatedParticipants = new HashSet<Participant>();

            // add all users that got their moderator state changed
            var updatedModerators = arg.Moderators.Except(_moderators).Concat(_moderators.Except(arg.Moderators))
                .Distinct();
            updatedParticipants.UnionWith(updatedModerators
                .Select(x => participants.FirstOrDefault(p => p.ParticipantId == x)).Where(x => x != null));

            if (!ComparePermissions(arg.ModeratorPermissions, _moderatorPermissions))
            {
                _moderatorPermissions = ParseDictionary(arg.ModeratorPermissions);
                updatedParticipants.UnionWith(arg.Moderators
                    .Select(x => participants.FirstOrDefault(p => p.ParticipantId == x))
                    .Where(x => x != null)); // add all current moderators
            }

            if (!ComparePermissions(arg.Permissions, _conferencePermissions))
            {
                _conferencePermissions = ParseDictionary(arg.Permissions);

                // add all participants of the conference
                updatedParticipants.UnionWith(participants);
            }

            _moderators = arg.Moderators;

            if (updatedParticipants.Any())
                await UpdatePermissions(updatedParticipants);
        }

        private async void OnDefaultPermissionChanged(DefaultPermissionOptions obj)
        {
            _logger.LogInformation("Default permissions updated. Update conference permissions.");
            _defaultPermissions = obj;

            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference != null)
                await OnConferenceUpdated(conference);
        }

        private static bool ComparePermissions(IReadOnlyDictionary<string, string>? source,
            IReadOnlyDictionary<string, JsonElement>? target)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null) return false;

            return source.EqualItems(target.ToDictionary(x => x.Key, x => x.Value.ToString()));
        }

        private static IImmutableDictionary<string, JsonElement>? ParseDictionary(
            IReadOnlyDictionary<string, string>? dictionary)
        {
            return dictionary?.ToImmutableDictionary(x => x.Key, x => JsonSerializer.Deserialize<JsonElement>(x.Value));
        }
    }
}
