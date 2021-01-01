using System;
using PaderConference.Core.Services;

namespace PaderConference.Core.Tests
{
    public class ConferenceOptionsWrapper<T> : IConferenceOptions<T> where T : class
    {
        private T _value;

        public ConferenceOptionsWrapper(T value)
        {
            _value = value;
        }

        public T Value
        {
            get => _value;
            set
            {
                Updated?.Invoke(this, new ObjectChangedEventArgs<T>(value, _value));
                _value = value;
            }
        }

        public event EventHandler<ObjectChangedEventArgs<T>>? Updated;
    }
}
