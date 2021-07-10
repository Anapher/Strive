using System.Collections.Generic;

namespace Strive.Infrastructure.KeyValue
{
    public class DatabaseKeyBuilder
    {
        private string? _conferenceId;
        private readonly List<string> _secondary;
        private readonly string _propertyKey;

        private DatabaseKeyBuilder(string propertyKey)
        {
            _propertyKey = propertyKey;
            _secondary = new List<string>();
        }

        public static DatabaseKeyBuilder ForProperty(string propertyKey)
        {
            return new(propertyKey);
        }

        public DatabaseKeyBuilder ForConference(string conferenceId)
        {
            _conferenceId = conferenceId;
            return this;
        }

        public DatabaseKeyBuilder ForSecondary(string secondary)
        {
            _secondary.Add(secondary);
            return this;
        }

        public override string ToString()
        {
            var segments = new List<string>();
            if (_conferenceId != null) segments.Add(_conferenceId);
            segments.AddRange(_secondary);
            segments.Add(_propertyKey);

            return string.Join(":", segments);
        }
    }
}
