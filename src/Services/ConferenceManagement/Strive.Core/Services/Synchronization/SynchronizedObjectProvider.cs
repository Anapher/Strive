using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Strive.Core.Services.Synchronization
{
    public abstract class SynchronizedObjectProvider<T> : ISynchronizedObjectProvider where T : class
    {
        public Type Type { get; } = typeof(T);

        public abstract string Id { get; }

        public abstract ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant);

        protected abstract ValueTask<T> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId);

        public async ValueTask<object> FetchValue(string conferenceId, SynchronizedObjectId synchronizedObjectId)
        {
            return await InternalFetchValue(conferenceId, synchronizedObjectId);
        }
    }
}
