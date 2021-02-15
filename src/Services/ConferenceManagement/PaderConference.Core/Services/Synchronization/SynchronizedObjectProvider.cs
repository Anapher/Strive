using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public abstract class SynchronizedObjectProvider<T> : ISynchronizedObjectProvider where T : class
    {
        public abstract string Id { get; }

        public abstract ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(string conferenceId,
            string participantId);

        protected abstract ValueTask<T> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId);

        public async ValueTask<object> FetchValue(string conferenceId, SynchronizedObjectId synchronizedObjectId)
        {
            return await InternalFetchValue(conferenceId, synchronizedObjectId);
        }
    }
}
