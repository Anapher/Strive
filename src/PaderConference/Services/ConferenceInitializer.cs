using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaderConference.Infrastructure;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Services
{
    public class ConferenceInitializer : IHostedService
    {
        private readonly ILogger<ConferenceInitializer> _logger;
        private readonly IRedisDatabase _redisDatabase;

        public ConferenceInitializer(IRedisDatabase redisDatabase, ILogger<ConferenceInitializer> logger)
        {
            _redisDatabase = redisDatabase;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // clear all conferences
            // as this program holds all participants etc., we couldn't possibly recover any conferences that existed before
            try
            {
                await _redisDatabase.PublishAsync<object?>(RedisChannels.OnResetConferences, null);
                await _redisDatabase.RemoveAsync(RedisKeys.OpenConferences);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Resetting redis failed.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}