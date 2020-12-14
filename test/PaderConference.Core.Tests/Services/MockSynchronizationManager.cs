using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Tests.Services
{
    public class MockSynchronizationManager : ISynchronizationManager
    {
        public readonly Dictionary<string, ISynchronizedObject> Objects = new Dictionary<string, ISynchronizedObject>();

        public ISynchronizedObject<T> Register<T>(string name, T initialValue, ParticipantGroup group) where T : notnull
        {
            var result = new MockSynchronizationObject<T>(initialValue);
            Objects[name] = result;

            return result;
        }
    }

    public class MockSynchronizationObject<T> : ISynchronizedObject<T>
    {
        public MockSynchronizationObject(T current)
        {
            Current = current;
        }

        public object GetCurrent()
        {
            return Current;
        }

        public T Current { get; private set; }

        public ValueTask Update(T newValue)
        {
            Current = newValue;
            return new ValueTask();
        }
    }
}
