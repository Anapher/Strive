using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Concurrency;
using MongoDB.Driver;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Infrastructure.Utilities;
using SpeciVacation;

namespace PaderConference.Infrastructure.Data.Repos
{
    public class ConferenceLinkRepo : MongoRepo<ConferenceLink>, IMongoIndexBuilder, IConferenceLinkRepo
    {
        public ConferenceLinkRepo(IOptions<MongoDbOptions> options) : base(options)
        {
        }

        static ConferenceLinkRepo()
        {
            BsonClassMap.RegisterClassMap<ConferenceLink>(config =>
            {
                config.AutoMap();
                config.SetIgnoreExtraElements(true);
            });
        }

        public async Task CreateIndexes()
        {
            await Collection.Indexes.CreateOneAsync(new CreateIndexModel<ConferenceLink>(
                Builders<ConferenceLink>.IndexKeys.Combine(
                    Builders<ConferenceLink>.IndexKeys.Ascending(x => x.ParticipantId),
                    Builders<ConferenceLink>.IndexKeys.Ascending(x => x.ConferenceId)),
                new CreateIndexOptions {Unique = true}));
        }

        public async Task<IReadOnlyList<ConferenceLink>> FindAsync(ISpecification<ConferenceLink> specification)
        {
            return await Collection.Find(new ExpressionFilterDefinition<ConferenceLink>(specification.ToExpression()))
                .ToListAsync();
        }

        public Task<OptimisticUpdateResult> CreateOrReplaceAsync(ConferenceLink conferenceLink)
        {
            return Collection.Optimistic(x => x.Version)
                .UpdateAsync(conferenceLink, new ReplaceOptions {IsUpsert = true}).Wrap();
        }

        public Task DeleteAsync(ConferenceLink conferenceLink)
        {
            return Collection.DeleteOneAsync(
                new ExpressionFilterDefinition<ConferenceLink>(x => x.Id == conferenceLink.Id));
        }
    }
}
