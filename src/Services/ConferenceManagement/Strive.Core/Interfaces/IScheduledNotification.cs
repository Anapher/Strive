using MediatR;

namespace Strive.Core.Interfaces
{
    public interface IScheduledNotification : INotification
    {
        string? TokenId { get; set; }
    }
}
