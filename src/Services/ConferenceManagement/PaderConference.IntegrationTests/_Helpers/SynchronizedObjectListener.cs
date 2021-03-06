using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
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
        private readonly List<AsyncAutoResetEvent> _waiters = new();

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

                var initialObj = initialEvent.Payload.ToObject<T>(JsonSerializer.Create(new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>
                    {
                        new ReadOnlyListConverter(), new ReadOnlyDictionaryConverter(),
                    },
                }));

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

        public Task<T> WaitForSyncObj<T>(SynchronizedObjectId syncObjId, TimeSpan? timeout = null) where T : class
        {
            return WaitForSyncObj<T>(syncObjId.ToString(), timeout);
        }

        public async Task<T> WaitForSyncObj<T>(string syncObjId, TimeSpan? timeout = null) where T : class
        {
            var timeoutTimestamp = DateTimeOffset.UtcNow.Add(timeout ?? TimeSpan.FromSeconds(5));

            var autoResetEvent = new AsyncAutoResetEvent(false);

            lock (_lock)
            {
                if (_cachedData.ContainsKey(syncObjId))
                    return GetSynchronizedObject<T>(syncObjId);

                _waiters.Add(autoResetEvent);
            }

            try
            {
                while (timeoutTimestamp > DateTimeOffset.UtcNow)
                {
                    var timeLeft = timeoutTimestamp - DateTimeOffset.UtcNow;
                    using (var tokenSource = new CancellationTokenSource(timeLeft))
                    {
                        await autoResetEvent.WaitAsync(tokenSource.Token);
                    }

                    lock (_lock)
                    {
                        if (_cachedData.ContainsKey(syncObjId))
                            return GetSynchronizedObject<T>(syncObjId);
                    }
                }
            }
            finally
            {
                lock (_logger)
                {
                    _waiters.Remove(autoResetEvent);
                }
            }

            throw new InvalidOperationException("The sync obj was never received");
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
            List<AsyncAutoResetEvent> autoResetEvents;
            lock (_lock)
            {
                if (!_cachedData.TryGetValue(value.Id, out var events))
                    _cachedData[value.Id] = events = new List<SyncObjEvent>();

                events.Add(new SyncObjEvent(isPatch, value.Value));
                autoResetEvents = _waiters.ToList();
            }

            foreach (var autoResetEvent in autoResetEvents)
            {
                autoResetEvent.Set();
            }
        }

        private record SyncObjEvent(bool IsPatch, JToken Payload);
    }
}
