using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PaderConference.Core.Services.Synchronization
{
    public readonly struct SynchronizedObjectId
    {
        public SynchronizedObjectId(string id, IReadOnlyDictionary<string, string> parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public SynchronizedObjectId(string id)
        {
            Id = id;
            Parameters = ImmutableDictionary<string, string>.Empty;
        }

        public string Id { get; }

        public IReadOnlyDictionary<string, string> Parameters { get; }

        public static SynchronizedObjectId Parse(string s)
        {
            var split = s.Split('?', 2);
            var id = split[0];
            var query = split.Length > 1 ? split[1] : string.Empty;

            var parameters = ParseQueryString(query);
            return new SynchronizedObjectId(id, parameters);
        }

        public override string ToString()
        {
            var text = $"{Id}";
            if (Parameters.Any()) text += "?" + BuildQueryString(Parameters);

            return text;
        }

        private static IReadOnlyDictionary<string, string> ParseQueryString(string s)
        {
            var parts = s.Split('&', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split('=', 2));
            return parts.ToDictionary(keyVal => keyVal[0], keyVal => keyVal[1]);
        }

        private static string BuildQueryString(IReadOnlyDictionary<string, string> parameters)
        {
            return string.Join('&', parameters.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}
