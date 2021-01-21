using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonPatchGenerator;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Synchronization.Serialization;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Synchronization
{
    /// <summary>
    ///     The synchronization service implements the pattern of synchronized objects other services may use. Synchronized
    ///     objects are immutable POCO classes with data that are always synchronized with their target group. When
    ///     synchronized objects are updated, a json patch is generated that will be sent to all clients this object is
    ///     synchronized to
    /// </summary>
    public class SynchronizationService : ConferenceService, ISynchronizationManager
    {
        private readonly ISignalMessenger _clients;
        private readonly string _conferenceId;
        private readonly IConferenceManager _conferenceManager;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<SynchronizationService> _logger;

        private readonly ConcurrentDictionary<string, SynchronizedObjectBase> _registeredObjects = new();
        private readonly ModeratorWatcher _moderatorWatcher;

        public SynchronizationService(ISignalMessenger clients, string conferenceId, IConferenceRepo conferenceRepo,
            IConferenceManager conferenceManager, IConnectionMapping connectionMapping,
            ILogger<SynchronizationService> logger)
        {
            _clients = clients;
            _conferenceId = conferenceId;
            _conferenceManager = conferenceManager;
            _connectionMapping = connectionMapping;
            _logger = logger;

            _moderatorWatcher = new ModeratorWatcher(conferenceId, conferenceRepo);
        }

        public override async ValueTask InitializeAsync()
        {
            await _moderatorWatcher.InitializeAsync();
            _moderatorWatcher.ModeratorsUpdated += ModeratorWatcherOnModeratorsUpdated;
        }

        public override async ValueTask DisposeAsync()
        {
            await _moderatorWatcher.DisposeAsync();
            _moderatorWatcher.ModeratorsUpdated -= ModeratorWatcherOnModeratorsUpdated;
        }

        public ISynchronizedObject<T> Register<T>(string name, T initialValue, ParticipantGroup group) where T : notnull
        {
            var obj = new SynchronizedObject<T>(name, initialValue, group, UpdateObject);

            if (!_registeredObjects.TryAdd(name, obj))
                throw new InvalidOperationException("An object with the same name was already registered.");

            _logger.LogDebug("Registered new synchronized object {name} for group {group}", name, group);

            return obj;
        }

        public override async ValueTask InitializeParticipant(Participant participant)
        {
            var connections = _connectionMapping.ConnectionsR[participant.ParticipantId];
            var state = GetState(participant);

            _logger.LogDebug("Initialize participant {participantId}, send state {@state}", participant.ParticipantId,
                state);

            await _clients.SendToConnectionAsync(connections.MainConnectionId,
                CoreHubMessages.Response.OnSynchronizeObjectState, state);
        }

        public IReadOnlyDictionary<string, object> GetState(Participant participant)
        {
            return _registeredObjects.Where(x => IsInGroup(participant, x.Value.ParticipantGroup))
                .ToDictionary(x => x.Key, x => x.Value.GetCurrent());
        }

        private async ValueTask UpdateObject<T>(SynchronizedObject<T> obj, T oldValue, T newValue) where T : notnull
        {
            _logger.LogDebug("Synchronized object {name} was updated.", obj.Name);

            var patch = JsonPatchFactory.CreatePatch(oldValue, newValue);
            if (!patch.Operations.Any()) return;

            var updateDto = new SynchronizedObjectUpdatedDto(obj.Name,
                patch.Operations.Select(x => new SerializableJsonPatchOperation(x)).ToList());

            await SendUpdate(obj.ParticipantGroup, updateDto);
        }

        private async void ModeratorWatcherOnModeratorsUpdated(object? sender, ModeratorUpdateInfo e)
        {
            _logger.LogDebug("Moderators have changed, update synchronized state for all changed moderators");

            // for all participants that got their moderator status changed, refresh the full state so synchronized objects
            // are removed or added
            foreach (var participantId in e.Added.Concat(e.Removed))
            {
                if (_conferenceManager.TryGetParticipant(_conferenceId, participantId, out var participant))
                    await InitializeParticipant(participant);
            }
        }

        private bool IsInGroup(Participant participant, ParticipantGroup group)
        {
            switch (group)
            {
                case ParticipantGroup.All:
                    return true;
                case ParticipantGroup.Moderators:
                    return _moderatorWatcher.Moderators.Contains(participant.ParticipantId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(group), group, null);
            }
        }

        private async Task SendUpdate(ParticipantGroup receiver, SynchronizedObjectUpdatedDto dto)
        {
            switch (receiver)
            {
                case ParticipantGroup.All:
                    await _clients.SendToConferenceAsync(_conferenceId,
                        CoreHubMessages.Response.OnSynchronizedObjectUpdated, dto);
                    break;
                case ParticipantGroup.Moderators:
                    foreach (var moderatorId in _moderatorWatcher.Moderators)
                    {
                        if (_connectionMapping.ConnectionsR.TryGetValue(moderatorId, out var connections))
                            await _clients.SendToConnectionAsync(connections.MainConnectionId,
                                CoreHubMessages.Response.OnSynchronizedObjectUpdated, dto);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(receiver), receiver, null);
            }
        }
    }
}