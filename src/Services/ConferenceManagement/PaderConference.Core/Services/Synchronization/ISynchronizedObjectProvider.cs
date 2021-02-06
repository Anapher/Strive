using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public interface ISynchronizedObjectProvider<T>
    {
        ValueTask<T> GetInitialValue(string conferenceId);

        string Name { get; }

        ParticipantGroup TargetGroup { get; }

        ValueTask<T> Update(string conferenceId, T newValue);

        ValueTask<T> Update(string conferenceId, Func<T, T> updateStateFn);
    }
}
