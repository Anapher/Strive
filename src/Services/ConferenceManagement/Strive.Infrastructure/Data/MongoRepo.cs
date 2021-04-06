using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Strive.Infrastructure.Data
{
    public abstract class MongoRepo<T>
    {
        protected readonly IMongoCollection<T> Collection;
        protected readonly MongoClient MongoClient;

        protected MongoRepo(IOptions<MongoDbOptions> options)
        {
            MongoClient = new MongoClient(options.Value.ConnectionString);
            var database = MongoClient.GetDatabase(options.Value.DatabaseName);
            Collection = database.GetCollection<T>(options.Value.CollectionNames[typeof(T).Name]);
        }
    }
}