using MediatR;

namespace PaderConference.Core.Services.Synchronization.Notifications
{
    public record SynchronizedObjectUpdatedNotification(string ConferenceId, string Name, object Value,
        object? PreviousValue) : INotification;
}
