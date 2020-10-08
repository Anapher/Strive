using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonPatchGenerator;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Serialization;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure.Services.Synchronization
{
    public class SynchronizationService : ConferenceService, ISynchronizationManager
    {
        private readonly IHubClients _clients;
        private readonly Conference _conference;
        private readonly IConnectionMapping _connectionMapping;

        private readonly ConcurrentDictionary<string, ISynchronizedObject> _registeredObjects =
            new ConcurrentDictionary<string, ISynchronizedObject>();

        public SynchronizationService(IHubClients clients, Conference conference,
            IConnectionMapping connectionMapping)
        {
            _clients = clients;
            _conference = conference;
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

        public override async ValueTask OnClientConnected(Participant participant)
        {
            var connectionId = _connectionMapping.ConnectionsR[participant];

            await _clients.Client(connectionId)
                .SendAsync(CoreHubMessages.Response.OnSynchronizeObjectState, GetState());
        }

        public IReadOnlyDictionary<string, object> GetState()
        {
            return _registeredObjects.ToDictionary(x => x.Key, x => x.Value.GetCurrent());
        }

        private async ValueTask UpdateObject<T>(T oldValue, T newValue, string name) where T : notnull
        {
            var patch = JsonPatchFactory.CreatePatch(oldValue, newValue);

            await _clients.Group(_conference.ConferenceId).SendAsync(
                CoreHubMessages.Response.OnSynchronizedObjectUpdated,
                new SynchronizedObjectUpdatedDto(name,
                    patch.Operations.Select(x => new SerializableJsonPatchOperation(x)).ToList()));
        }
    }
}