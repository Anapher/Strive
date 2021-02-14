namespace PaderConference.Core.Services.Synchronization.UpdateStrategy
{
    public class ReplaceValueUpdate<T> : IValueUpdate<T>
    {
        public ReplaceValueUpdate(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
