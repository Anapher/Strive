using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonPatchGenerator;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Synchronization.Serialization;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Synchronization
{
    public class SynchronizationService : ConferenceService, ISynchronizationManager
    {
        private readonly ISignalMessenger _clients;
        private readonly string _conferenceId;
        private readonly IConnectionMapping _connectionMapping;

        private readonly ConcurrentDictionary<string, ISynchronizedObject> _registeredObjects =
            new ConcurrentDictionary<string, ISynchronizedObject>();

        public SynchronizationService(ISignalMessenger clients, string conferenceId,
            IConnectionMapping connectionMapping)
        {
            _clients = clients;
            _conferenceId = conferenceId;
            _connectionMapping = connectionMapping;
        }

        public ISynchronizedObject<T> Register<T>(string name, T initialValue) where T : notnull
        {
            var obj = new SynchronizedObject<T>(initialValue,
                (oldValue, newValue) => UpdateObject(oldValue, newValue, name));

            if (!_registeredObjects.TryAdd(name, obj))
                throw new InvalidOperationException("An object with the same name was already registered.");

            return obj;
        }

        public override async ValueTask InitializeParticipant(Participant participant)
        {
            var connections = _connectionMapping.ConnectionsR[participant.ParticipantId];
            var state = GetState();

            await _clients.SendToConnectionAsync(connections.MainConnectionId,
                CoreHubMessages.Response.OnSynchronizeObjectState, state);
        }

        public IReadOnlyDictionary<string, object> GetState()
        {
            return _registeredObjects.ToDictionary(x => x.Key, x => x.Value.GetCurrent());
        }

        private async ValueTask UpdateObject<T>(T oldValue, T newValue, string name) where T : notnull
        {
            var patch = JsonPatchFactory.CreatePatch(oldValue, newValue);
            if (!patch.Operations.Any()) return;

            await _clients.SendToConferenceAsync(_conferenceId, CoreHubMessages.Response.OnSynchronizedObjectUpdated,
                new SynchronizedObjectUpdatedDto(name,
                    patch.Operations.Select(x => new SerializableJsonPatchOperation(x)).ToList()));
        }
    }
}