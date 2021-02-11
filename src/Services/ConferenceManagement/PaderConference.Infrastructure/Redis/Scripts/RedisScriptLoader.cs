using System;
using System.Collections.Generic;
using System.Reflection;
using PaderConference.Infrastructure.Utilities;

namespace PaderConference.Infrastructure.Redis.Scripts
{
    public static class RedisScriptLoader
    {
        private static readonly IReadOnlyDictionary<RedisScript, string> _scripts;

        static RedisScriptLoader()
        {
            var assembly = typeof(RedisScriptLoader).GetTypeInfo().Assembly;
            var fileNamespace = typeof(RedisScriptLoader).Namespace;

            var scripts = new Dictionary<RedisScript, string>();
            foreach (var scriptKey in Enum.GetValues<RedisScript>())
            {
                var scriptPath = $"{fileNamespace}.{scriptKey}.lua";
                scripts[scriptKey] = EmbeddedResourceUtils.LoadResourceFile(assembly, scriptPath);
            }

            _scripts = scripts;
        }

        public static string Load(RedisScript script)
        {
            return _scripts[script];
        }
    }

    public enum RedisScript
    {
        JoinedParticipantsRepository_RemoveParticipant,
        JoinedParticipantsRepository_RemoveParticipantSafe,
        RoomRepository_SetParticipantRoom,
    }
}
