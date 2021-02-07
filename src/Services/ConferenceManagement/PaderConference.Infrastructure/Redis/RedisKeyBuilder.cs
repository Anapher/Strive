using System.Collections.Generic;

namespace PaderConference.Infrastructure.Redis
{
    public class RedisKeyBuilder
    {
        private string? _conferenceId;
        private string? _participantId;
        private readonly string _propertyKey;

        private RedisKeyBuilder(string propertyKey)
        {
            _propertyKey = propertyKey;
        }

        public static RedisKeyBuilder ForProperty(string propertyKey)
        {
            return new(propertyKey);
        }

        public RedisKeyBuilder ForConference(string conferenceId)
        {
            _conferenceId = conferenceId;
            return this;
        }

        public RedisKeyBuilder ForParticipant(string participantId)
        {
            _participantId = participantId;
            return this;
        }

        public override string ToString()
        {
            var segments = new List<string>();
            if (_conferenceId != null) segments.Add(_conferenceId);
            if (_participantId != null) segments.Add(_participantId);
            segments.Add(_propertyKey);

            return string.Join(":", segments);
        }
    }
}
