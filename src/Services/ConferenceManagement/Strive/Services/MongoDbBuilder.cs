using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Strive.Infrastructure.Data;

namespace Strive.Services
{
    public class MongoDbBuilder : IHostedService
    {
        private readonly IEnumerable<IMongoIndexBuilder> _indexBuilders;

        public MongoDbBuilder(IEnumerable<IMongoIndexBuilder> indexBuilders)
        {
            _indexBuilders = indexBuilders;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var indexBuilder in _indexBuilders)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await indexBuilder.CreateIndexes();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}