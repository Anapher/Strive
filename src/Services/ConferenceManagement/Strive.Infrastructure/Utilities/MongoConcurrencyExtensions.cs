using System.Threading.Tasks;
using MongoDB.Concurrency.Optimistic;
using MongoDB.Driver;
using Strive.Core.Interfaces.Gateways;

namespace Strive.Infrastructure.Utilities
{
    public static class MongoConcurrencyExtensions
    {
        public static async Task<OptimisticUpdateResult> Wrap(this Task<ReplaceOneResult?> task)
        {
            try
            {
                await task;
                return OptimisticUpdateResult.Ok;
            }
            catch (MongoConcurrencyUpdatedException)
            {
                return OptimisticUpdateResult.ConcurrencyException;
            }
            catch (MongoConcurrencyDeletedException)
            {
                return OptimisticUpdateResult.DeletedException;
            }
        }
    }
}
