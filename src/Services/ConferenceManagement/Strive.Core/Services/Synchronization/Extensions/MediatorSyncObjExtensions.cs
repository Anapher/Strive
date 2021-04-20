using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Synchronization.Extensions
{
    public static class MediatorSyncObjExtensions
    {
        public static async Task<T> FetchSynchronizedObject<T>(this IMediator mediator, string conferenceId,
            SynchronizedObjectId id) where T : class
        {
            return (T) await mediator.Send(new FetchSynchronizedObjectRequest(conferenceId, id));
        }
    }
}
