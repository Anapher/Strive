using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderConference.Core.Domain.Entities
{
    public class User
    {
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        public User(string id, string userName, string passwordHash)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
        }

        public string Id { get; }
        public string UserName { get; }
        public string? Email { get; set; }
        public string PasswordHash { get; set; }
        public string? UnconfirmedEmailAddress { get; set; }

        public bool HasConfirmedEmailAddress() => Email != null;

        public bool HasValidRefreshToken(string refreshToken)
        {
            return _refreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);
        }

        public void AddRefreshToken(string token, string? remoteIpAddress, double daysToExpire = 5)
        {
            _refreshTokens.Add(new RefreshToken(token, DateTimeOffset.UtcNow.AddDays(daysToExpire), remoteIpAddress));
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
        }
    }
}
