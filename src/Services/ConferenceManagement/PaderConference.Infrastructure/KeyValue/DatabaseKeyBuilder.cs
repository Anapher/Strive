using System.Collections.Generic;

namespace PaderConference.Infrastructure.KeyValue
{
    public class DatabaseKeyBuilder
    {
        private string? _conferenceId;
        private string? _secondary;
        private readonly string _propertyKey;

        private DatabaseKeyBuilder(string propertyKey)
        {
            _propertyKey = propertyKey;
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
            _secondary = secondary;
            return this;
        }

        public override string ToString()
        {
            var segments = new List<string>();
            if (_conferenceId != null) segments.Add(_conferenceId);
            if (_secondary != null) segments.Add(_secondary);
            segments.Add(_propertyKey);

            return string.Join(":", segments);
        }
    }
}
