using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Concurrency;
using MongoDB.Driver;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Infrastructure.Utilities;
using StackExchange.Redis.Extensions.Core.Abstractions;

#pragma warning disable 8619

namespace PaderConference.Infrastructure.Data.Repos
{
    public class ConferenceRepo : MongoRepo<Conference>, IConferenceRepo
    {
        private readonly IRedisDatabase _database;

        static ConferenceRepo()
        {
            BsonClassMap.RegisterClassMap<Conference>(config =>
            {
                config.AutoMap();
                config.MapIdMember(x => x.ConferenceId);
            });
        }

        public ConferenceRepo(IOptions<MongoDbOptions> options, IRedisDatabase database) : base(options)
        {
            _database = database;
        }

        public Task<Conference?> FindById(string conferenceId)
        {
            return Collection.Find(x => x.ConferenceId == conferenceId).FirstOrDefaultAsync();
        }

        public Task Create(Conference conference)
        {
            return Collection.InsertOneAsync(conference);
        }

        public Task<OptimisticUpdateResult> Update(Conference conference)
        {
            return Collection.Optimistic(x => x.Version).UpdateAsync(conference).Wrap();
        }

        public async Task<IAsyncDisposable> SubscribeConferenceUpdated(string conferenceId,
            Func<Conference, Task> handler)
        {
            var channelName = RedisChannels.OnConferenceUpdated(conferenceId);
            await _database.SubscribeAsync(channelName, handler);

            return new DelegateAsyncDisposable(() => _database.UnsubscribeAsync(channelName, handler));
        }
    }
}
