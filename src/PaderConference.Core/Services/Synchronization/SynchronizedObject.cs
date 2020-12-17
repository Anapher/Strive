using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public delegate ValueTask ApplyNewValueDelegate<T>(SynchronizedObject<T> sender, T oldValue, T newValue)
        where T : notnull;

    public abstract class SynchronizedObjectBase : ISynchronizedObject
    {
        public abstract string Name { get; }

        public abstract ParticipantGroup ParticipantGroup { get; }

        public abstract object GetCurrent();
    }

    public class SynchronizedObject<T> : SynchronizedObjectBase, ISynchronizedObject<T> where T : notnull
    {
        private readonly ApplyNewValueDelegate<T> _applyNewValue;
        private readonly object _updateLock = new();

        public SynchronizedObject(string name, T initialValue, ParticipantGroup participantGroup,
            ApplyNewValueDelegate<T> applyNewValue)
        {
            Name = name;
            Current = initialValue;
            ParticipantGroup = participantGroup;
            _applyNewValue = applyNewValue;
        }

        public override string Name { get; }

        public override ParticipantGroup ParticipantGroup { get; }

        public T Current { get; private set; }

        public override object GetCurrent()
        {
            return Current;
        }

        public ValueTask Update(T newValue)
        {
            T oldState;

            lock (_updateLock)
            {
                oldState = Current;
                Current = newValue;
            }

            Current = newValue;
            return _applyNewValue(this, oldState, newValue);
        }

        public async ValueTask<T> Update(Func<T, T> updateStateFn)
        {
            T oldState;
            T newState;

            lock (_updateLock)
            {
                oldState = Current;
                newState = updateStateFn(oldState);

                Current = newState;
            }

            await _applyNewValue(this, oldState, newState);
            return newState;
        }
    }
}