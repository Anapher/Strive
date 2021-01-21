using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Services
{
    public class ConferenceInitializer : IHostedService
    {
        private readonly ILogger<ConferenceInitializer> _logger;
        private readonly IOpenConferenceRepo _repo;

        public ConferenceInitializer(IOpenConferenceRepo repo, ILogger<ConferenceInitializer> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // clear all conferences
            // as this program holds all participants etc., we couldn't possibly recover any conferences that existed before
            try
            {
                await _repo.DeleteAll();
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