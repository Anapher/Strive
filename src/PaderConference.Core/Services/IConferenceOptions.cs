using System;

namespace PaderConference.Core.Services
{
    public interface IConferenceOptions<T> where T : class
    {
        T Value { get; }

        event EventHandler<ObjectChangedEventArgs<T>>? Updated;
    }
}
