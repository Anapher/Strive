using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Extensions;
using PaderConference.Core.Utilities;
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

        public virtual ValueTask<bool> KeyDeleteAsync(string key)
        {
            using (Lock())
            {
                return new ValueTask<bool>(_data.Remove(key));
            }
        }

        public virtual ValueTask<string?> HashGetAsync(string key, string field)
        {
            using (LockRead())
            {
                if (_data.TryGetValue(key, out var hashSetObj))
                {
                    var hashSet = (Dictionary<string, string>) hashSetObj;
                    if (hashSet.TryGetValue(field, out var value))
                        return new ValueTask<string?>(value);
                }
            }

            return new ValueTask<string?>((string?) null);
        }

        public virtual ValueTask HashSetAsync(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            using (LockRead())
            {
                if (!_data.TryGetValue(key, out var hashSetObj))
                    _data[key] = hashSetObj = new Dictionary<string, string>();

                var hashSet = (Dictionary<string, string>) hashSetObj;
                foreach (var (field, value) in keyValuePairs)
                {
                    hashSet[field] = value;
                }
            }

            return new ValueTask();
        }

        public virtual ValueTask HashSetAsync(string key, string field, string value)
        {
            return HashSetAsync(key, new KeyValuePair<string, string>(field, value).Yield());
        }

        public virtual ValueTask<bool> HashExistsAsync(string key, string field)
        {
            using (LockRead())
            {
                if (!_data.TryGetValue(key, out var hashSetObj))
                    return new ValueTask<bool>(false);

                var hashSet = (Dictionary<string, string>) hashSetObj;
                return new ValueTask<bool>(hashSet.ContainsKey(field));
            }
        }

        public virtual ValueTask<bool> HashDeleteAsync(string key, string field)
        {
            using (Lock())
            {
                if (!_data.TryGetValue(key, out var hashSetObj))
                    return new ValueTask<bool>(false);

                var hashSet = (Dictionary<string, string>) hashSetObj;
                if (hashSet.Remove(field))
                {
                    if (!hashSet.Any()) _data.Remove(key);

                    return new ValueTask<bool>(true);
                }

                return new ValueTask<bool>(false);
            }
        }

        public virtual ValueTask<IReadOnlyDictionary<string, string>> HashGetAllAsync(string key)
        {
            using (LockRead())
            {
                if (!_data.TryGetValue(key, out var hashSetObj))
                    return new ValueTask<IReadOnlyDictionary<string, string>>(ImmutableDictionary<string, string>
                        .Empty);

                var hashSet = (Dictionary<string, string>) hashSetObj;
                return new ValueTask<IReadOnlyDictionary<string, string>>(
                    hashSet.ToImmutableDictionary(x => x.Key, x => x.Value));
            }
        }

        public virtual ValueTask<string?> GetAsync(string key)
        {
            using (LockRead())
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

        public virtual ValueTask ListRightPushAsync(string key, string item)
        {
            using (Lock())
            {
                if (!_data.TryGetValue(key, out var listObj))
                    _data[key] = listObj = new List<string>();

                var list = (List<string>) listObj;
                list.Add(item);

                return new ValueTask();
            }
        }

        public virtual ValueTask<int> ListLenAsync(string key)
        {
            using (LockRead())
            {
                if (!_data.TryGetValue(key, out var listObj))
                    return new ValueTask<int>(0);

                var list = (List<string>) listObj;
                return new ValueTask<int>(list.Count);
            }
        }

        public virtual ValueTask<IReadOnlyList<string>> ListRangeAsync(string key, int start, int end)
        {
            using (LockRead())
            {
                if (!_data.TryGetValue(key, out var listObj))
                    return new ValueTask<IReadOnlyList<string>>(ImmutableList<string>.Empty);

                var list = (List<string>) listObj;
                if (!list.Any())
                    return new ValueTask<IReadOnlyList<string>>(ImmutableList<string>.Empty);

                (start, end) = IndexUtils.TranslateStartEndIndex(start, end, list.Count);
                if (start > list.Count) return new ValueTask<IReadOnlyList<string>>(ImmutableList<string>.Empty);

                var rangeLength = end - start + 1;
                var range = list.Skip(start).Take(rangeLength).ToList();
                return new ValueTask<IReadOnlyList<string>>(range);
            }
        }

        public virtual ValueTask<bool> SetAddAsync(string key, string value)
        {
            using (Lock())
            {
                if (!_data.TryGetValue(key, out var setObj))
                    _data[key] = setObj = new HashSet<string>();

                var set = (HashSet<string>) setObj;
                var added = set.Add(value);
                return new ValueTask<bool>(added);
            }
        }

        public virtual ValueTask<bool> SetRemoveAsync(string key, string value)
        {
            using (Lock())
            {
                if (!_data.TryGetValue(key, out var setObj))
                    return new ValueTask<bool>(false);

                var set = (HashSet<string>) setObj;
                var removed = set.Remove(value);

                if (removed && !set.Any())
                    _data.Remove(key);

                return new ValueTask<bool>(removed);
            }
        }

        public virtual ValueTask<IReadOnlyList<string>> SetMembersAsync(string key)
        {
            using (LockRead())
            {
                if (!_data.TryGetValue(key, out var setObj))
                    return new ValueTask<IReadOnlyList<string>>(ImmutableList<string>.Empty);

                var set = (HashSet<string>) setObj;
                return new ValueTask<IReadOnlyList<string>>(set.ToList());
            }
        }

        public virtual ValueTask<RedisResult> ExecuteScriptAsync(RedisScript script, params string[] parameters)
        {
            var actions = new NoLockInMemoryDatabaseActions(_data);

            switch (script)
            {
                case RedisScript.JoinedParticipantsRepository_RemoveParticipant:
                    return JoinedParticipantsRepository_RemoveParticipant(actions, parameters[0], parameters[1],
                        parameters[2]);
                case RedisScript.JoinedParticipantsRepository_RemoveParticipantSafe:
                    return JoinedParticipantsRepository_RemoveParticipantSafe(actions, parameters[0], parameters[1],
                        parameters[2], parameters[3]);
                case RedisScript.RoomRepository_SetParticipantRoom:
                    return RoomRepository_SetParticipantRoom(actions, parameters[0], parameters[1], parameters[2],
                        parameters[3]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(script), script, null);
            }
        }

        protected abstract IDisposable Lock();

        protected virtual IDisposable LockRead()
        {
            return Lock();
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
            var roomExists = await actions.HashExistsAsync(roomListKey, newRoomId);
            if (!roomExists) return RedisResult.Create(false);

            await actions.HashSetAsync(roomMappingKey, participantId, newRoomId);

            return RedisResult.Create(true);
        }
    }
}
