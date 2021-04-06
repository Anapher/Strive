using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Concurrency;
using MongoDB.Driver;
using Strive.Core.Domain.Entities;
using Strive.Core.Interfaces.Gateways;
using Strive.Core.Services.Chat;
using Strive.Core.Services.ConferenceManagement.Gateways;
using Strive.Infrastructure.Utilities;

#pragma warning disable 8619

namespace Strive.Infrastructure.Data.Repos
{
    public class ConferenceRepo : MongoRepo<Conference>, IConferenceRepo
    {
        static ConferenceRepo()
        {
            BsonClassMap.RegisterClassMap<Conference>(config =>
            {
                config.AutoMap();
                config.MapIdMember(x => x.ConferenceId);
                config.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ChatOptions>(config =>
            {
                config.AutoMap();
                config.SetIgnoreExtraElements(true);
            });
        }

        public ConferenceRepo(IOptions<MongoDbOptions> options) : base(options)
        {
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
    }
}
