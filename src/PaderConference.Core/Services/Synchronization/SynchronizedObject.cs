using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public class SynchronizedObject<T> : ISynchronizedObject<T> where T : notnull
    {
        private readonly Func<T, T, ValueTask> _applyNewValue;

        public SynchronizedObject(T initialValue, Func<T, T, ValueTask> applyNewValue)
        {
            _applyNewValue = applyNewValue;
            Current = initialValue;
        }

        public T Current { get; private set; }

        public object GetCurrent()
        {
            return Current;
        }

        public ValueTask Update(T newValue)
        {
            var oldValue = Current;

            Current = newValue;
            return _applyNewValue(oldValue, newValue);
        }
    }
}