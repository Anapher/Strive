using System;

namespace PaderConference.Core.Domain.Entities
{
    public class RefreshToken
    {
        public RefreshToken(string userId, string token, DateTimeOffset expires)
        {
            UserId = userId;
            Value = token;
            Expires = expires;
        }

        public string UserId { get; init; }

        public string Value { get; init; }

        public DateTimeOffset Expires { get; init; }

        public bool Active => DateTimeOffset.UtcNow <= Expires;
    }
}
