using Strive.Core.Interfaces;

namespace Strive.Core.Services.BreakoutRooms.Notifications
{
    public record BreakoutRoomTimerElapsedNotification(string ConferenceId) : IScheduledNotification
    {
        public string? TokenId { get; set; }
    }
}
