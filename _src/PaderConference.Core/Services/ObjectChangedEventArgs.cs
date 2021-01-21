namespace PaderConference.Core.Services
{
    public record ObjectChangedEventArgs<T>(T NewValue, T OldValue);
}
