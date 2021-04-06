using System.Threading.Tasks;

namespace Strive.Infrastructure.Data
{
    public interface IMongoIndexBuilder
    {
        Task CreateIndexes();
    }
}
