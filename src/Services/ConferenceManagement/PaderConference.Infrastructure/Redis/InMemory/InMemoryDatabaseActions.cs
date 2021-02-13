using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using PaderConference.Core.Extensions;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.InMemory
{
    public abstract class InMemoryDatabaseActions : IKeyValueDatabaseActions
    {
        private readonly Dictionary<string, object> _data;

        protected InMemoryDatabaseActions(Dictionary<string, object> data)
        {
            _data = data;
        }

        protected abstract IDisposable Lock();

        public virtual ValueTask<bool> KeyDeleteAsync(string key)
        {
            using (Lock())
            {
                return new ValueTask<bool>(_data.Remove(key));
            }
        }

        public virtual ValueTask<string?> HashGetAsync(string hashKey, string key)
        {
            using (Lock())
            {
                if (_data.TryGetValue(hashKey, out var hashSetObj))
                {
                    var hashSet = (Dictionary<string, string>) hashSetObj;
                    if (hashSet.TryGetValue(key, out var value))
                        return new ValueTask<string?>(value);
                }
            }

            return new ValueTask<string?>((string?) null);
        }

        public virtual ValueTask HashSetAsync(string hashKey, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            using (Lock())
            {
                if (!_data.TryGetValue(hashKey, out var hashSetObj))
                    _data[hashKey] = hashSetObj = new Dictionary<string, string>();

                var hashSet = (Dictionary<string, string>) hashSetObj;
                foreach (var (key, value) in keyValuePairs)
                {
                    hashSet[key] = value;
                }
            }

            return new ValueTask();
        }

        public virtual ValueTask HashSetAsync(string hashKey, string key, string value)
        {
            return HashSetAsync(hashKey, new KeyValuePair<string, string>(key, value).Yield());
        }

        public virtual ValueTask<bool> HashExists(string hashKey, string key)
        {
            using (Lock())
            {
                if (!_data.TryGetValue(hashKey, out var hashSetObj))
                    return new ValueTask<bool>(false);

                var hashSet = (Dictionary<string, string>) hashSetObj;
                return new ValueTask<bool>(hashSet.ContainsKey(key));
            }
        }

        public virtual ValueTask<bool> HashDeleteAsync(string hashKey, string key)
        {
            using (Lock())
            {
                if (!_data.TryGetValue(hashKey, out var hashSetObj))
                    return new ValueTask<bool>(false);

                var hashSet = (Dictionary<string, string>) hashSetObj;
                return new ValueTask<bool>(hashSet.Remove(key));
            }
        }

        public virtual ValueTask<IReadOnlyDictionary<string, string>> HashGetAllAsync(string hashKey)
        {
            using (Lock())
            {
                if (!_data.TryGetValue(hashKey, out var hashSetObj))
                    return new ValueTask<IReadOnlyDictionary<string, string>>(ImmutableDictionary<string, string>
                        .Empty);

                var hashSet = (Dictionary<string, string>) hashSetObj;
                return new ValueTask<IReadOnlyDictionary<string, string>>(
                    hashSet.ToImmutableDictionary(x => x.Key, x => x.Value));
            }
        }

        public virtual ValueTask<string?> GetAsync(string key)
        {
            using (Lock())
            {
                if (_data.TryGetValue(key, out var value))
                    return new ValueTask<string?>((string) value);

                return new ValueTask<string?>((string?) null);
            }
        }

        public virtual ValueTask<string?> GetSetAsync(string key, string value)
        {
            using (Lock())
            {
                _data.TryGetValue(key, out var dbValue);
                _data[key] = value;

                return new ValueTask<string?>((string?) dbValue);
            }
        }

        public virtual ValueTask SetAsync(string key, string value)
        {
            using (Lock())
            {
                _data[key] = value;
                return new ValueTask();
            }
        }

        public virtual ValueTask<RedisResult> ExecuteScriptAsync(RedisScript script, params object[] parameters)
        {
            var actions = new NoLockInMemoryDatabaseActions(_data);

            switch (script)
            {
                case RedisScript.JoinedParticipantsRepository_RemoveParticipant:
                    return JoinedParticipantsRepository_RemoveParticipant(actions, (string) parameters[0],
                        (string) parameters[1], (string) parameters[2]);
                case RedisScript.JoinedParticipantsRepository_RemoveParticipantSafe:
                    return JoinedParticipantsRepository_RemoveParticipantSafe(actions, (string) parameters[0],
                        (string) parameters[1], (string) parameters[2], (string) parameters[3]);
                case RedisScript.RoomRepository_SetParticipantRoom:
                    return RoomRepository_SetParticipantRoom(actions, (string) parameters[0], (string) parameters[1],
                        (string) parameters[2], (string) parameters[3]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(script), script, null);
            }
        }

        private static async ValueTask<RedisResult> JoinedParticipantsRepository_RemoveParticipant(
            IKeyValueDatabaseActions actions, string participantId, string participantKey, string conferenceKeyTemplate)
        {
            var conferenceId = await actions.GetAsync(participantKey);
            if (conferenceId == null) return RedisResult.Create(RedisValue.Null);

            await actions.KeyDeleteAsync(participantKey);

            var conferenceKey = conferenceKeyTemplate.Replace("*", conferenceId);
            var previousConnectionId = await actions.HashGetAsync(conferenceKey, participantId);

            await actions.HashDeleteAsync(conferenceKey, participantId);

            return RedisResult.Create(new RedisValue[] {conferenceId, previousConnectionId});
        }

        private static async ValueTask<RedisResult> JoinedParticipantsRepository_RemoveParticipantSafe(
            IKeyValueDatabaseActions actions, string participantId, string participantKey, string conferenceKeyTemplate,
            string connectionId)
        {
            var conferenceId = await actions.GetAsync(participantKey);
            if (conferenceId == null) return RedisResult.Create(RedisValue.Null);

            var conferenceKey = conferenceKeyTemplate.Replace("*", conferenceId);
            var actualConnectionId = await actions.HashGetAsync(conferenceKey, participantId);

            if (actualConnectionId == connectionId)
            {
                await actions.KeyDeleteAsync(participantKey);
                await actions.HashDeleteAsync(conferenceKey, participantId);

                return RedisResult.Create(true);
            }

            return RedisResult.Create(false);
        }

        private static async ValueTask<RedisResult> RoomRepository_SetParticipantRoom(IKeyValueDatabaseActions actions,
            string roomMappingKey, string roomListKey, string participantId, string newRoomId)
        {
            var roomExists = await actions.HashExists(roomListKey, newRoomId);
            if (!roomExists) return RedisResult.Create(false);

            await actions.HashSetAsync(roomMappingKey, participantId, newRoomId);

            return RedisResult.Create(true);
        }
    }
}
