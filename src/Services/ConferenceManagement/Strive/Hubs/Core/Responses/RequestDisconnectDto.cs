using Strive.Core.Services.ConferenceControl.Notifications;

namespace Strive.Hubs.Core.Responses
{
    public record RequestDisconnectDto(ParticipantKickedReason Reason);
}
