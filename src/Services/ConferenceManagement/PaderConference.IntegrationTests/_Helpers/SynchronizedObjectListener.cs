using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Hubs;
using Serilog;

namespace PaderConference.IntegrationTests._Helpers
{
    public class SynchronizedObjectListener
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, List<SyncObjEvent>> _cachedData = new();
        private readonly object _lock = new();

        private SynchronizedObjectListener(HubConnection connection, ILogger logger)
        {
            _logger = logger;

            connection.On<SyncObjPayload<JToken>>(CoreHubMessages.Response.OnSynchronizedObjectUpdated,
                HandleSynchronizedObjectUpdated);
            connection.On<SyncObjPayload<JToken>>(CoreHubMessages.Response.OnSynchronizeObjectState,
                HandleSynchronizeObjectState);
        }

        public static SynchronizedObjectListener Initialize(HubConnection connection, ILogger logger)
        {
            logger.Information("Init sync obj listener");
            return new(connection, logger);
        }

        public T GetSynchronizedObject<T>(SynchronizedObjectId syncObjId) where T : class
        {
            return GetSynchronizedObject<T>(syncObjId.ToString());
        }

        public T GetSynchronizedObject<T>(string syncObjId) where T : class
        {
            lock (_lock)
            {
                if (!_cachedData.TryGetValue(syncObjId, out var events))
                    throw new InvalidOperationException("The synchronized object was never received.");

                var initialEvent = events.Last(x => !x.IsPatch);
                var patches = events.Skip(events.IndexOf(initialEvent) + 1);

                var initialObj = initialEvent.Payload.ToObject<T>();
                if (initialObj == null) throw new NullReferenceException("The initial object must not be null");

                foreach (var patch in patches.Select(x => x.Payload))
                {
                    var jsonPatch = patch.ToObject<JsonPatchDocument<T>>();
                    if (jsonPatch == null) throw new NullReferenceException("A patch must never be null.");

                    jsonPatch.ApplyTo(initialObj);
                }

                return initialObj;
            }
        }

        private void HandleSynchronizedObjectUpdated(SyncObjPayload<JToken> value)
        {
            _logger.Information("Synchronized object patched {syncObjId}.", value.Id);
            AddEvent(value, true);
        }

        private void HandleSynchronizeObjectState(SyncObjPayload<JToken> value)
        {
            _logger.Information("Synchronized object {syncObjId} received.", value.Id);
            AddEvent(value, false);
        }

        private void AddEvent(SyncObjPayload<JToken> value, bool isPatch)
        {
            lock (_lock)
            {
                if (!_cachedData.TryGetValue(value.Id, out var events))
                    _cachedData[value.Id] = events = new List<SyncObjEvent>();

                events.Add(new SyncObjEvent(isPatch, value.Value));
            }
        }

        private record SyncObjEvent(bool IsPatch, JToken Payload);
    }
}
