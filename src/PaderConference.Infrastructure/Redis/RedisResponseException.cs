using System;

namespace PaderConference.Infrastructure.Redis
{
    public class RedisResponseException : Exception
    {
        public RedisResponseException(string? message) : base(message)
        {
        }
    }
}
