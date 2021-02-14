using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public abstract class SynchronizedObjectProvider<T> : ISynchronizedObjectProvider where T : class
    {
        public Type SynchronizedObjectType { get; } = typeof(T);

        public abstract ValueTask<bool> CanSubscribe(string conferenceId, string participantId);

        public async ValueTask<object> FetchValue(string conferenceId, string participantId)
        {
            return await InternalFetchValue(conferenceId, participantId);
        }

        protected abstract ValueTask<T> InternalFetchValue(string conferenceId, string participantId);

        public abstract ValueTask<string> GetSynchronizedObjectId(string conferenceId, string participantId);
    }
}
