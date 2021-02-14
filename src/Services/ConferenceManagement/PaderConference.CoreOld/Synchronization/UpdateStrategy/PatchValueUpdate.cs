using System;

namespace PaderConference.Core.Services.Synchronization.UpdateStrategy
{
    public class PatchValueUpdate<T> : IValueUpdate<T>
    {
        public Func<T?, T> UpdateStateFn { get; }

        public PatchValueUpdate(Func<T?, T> updateStateFn)
        {
            UpdateStateFn = updateStateFn;
        }
    }
}
