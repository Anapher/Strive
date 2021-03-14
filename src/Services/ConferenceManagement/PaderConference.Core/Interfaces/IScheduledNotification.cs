using MediatR;

namespace PaderConference.Core.Interfaces
{
    public interface IScheduledNotification : INotification
    {
        string? TokenId { get; set; }
    }
}
