#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Infrastructure.Data.Repos
{
    public class RefreshTokenRepo : MongoRepo<RefreshToken>, IRefreshTokenRepo, IMongoIndexBuilder
    {
        static RefreshTokenRepo()
        {
            BsonClassMap.RegisterClassMap<RefreshToken>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }

        public RefreshTokenRepo(IOptions<MongoDbOptions> options) : base(options)
        {
        }

        public Task PushRefreshToken(RefreshToken token)
        {
            return Collection.InsertOneAsync(token);
        }

        public Task<RefreshToken?> TryPopRefreshToken(string userId, string refreshToken)
        {
            return Collection.FindOneAndDeleteAsync(x => x.UserId == userId && x.Value == refreshToken);
        }

        public async Task CreateIndexes()
        {
            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<RefreshToken>(Builders<RefreshToken>.IndexKeys.Ascending(x => x.Expires)));

            await Collection.Indexes.CreateOneAsync(new CreateIndexModel<RefreshToken>(
                Builders<RefreshToken>.IndexKeys.Combine(Builders<RefreshToken>.IndexKeys.Ascending(x => x.UserId),
                    Builders<RefreshToken>.IndexKeys.Ascending(x => x.Value))));
        }
    }
}
