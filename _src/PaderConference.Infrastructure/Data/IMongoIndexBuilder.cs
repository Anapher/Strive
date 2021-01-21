using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Data
{
    public interface IMongoIndexBuilder
    {
        Task CreateIndexes();
    }
}
