using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Services.BreakoutRooms.Notifications
{
    public record BreakoutRoomTimerElapsedNotification(string ConferenceId) : IScheduledNotification
    {
        public string? TokenId { get; set; }
    }
}
