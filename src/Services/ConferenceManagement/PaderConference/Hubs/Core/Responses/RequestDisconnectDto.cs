using PaderConference.Core.Services.ConferenceControl.Notifications;

namespace PaderConference.Hubs.Core.Responses
{
    public record RequestDisconnectDto(ParticipantKickedReason Reason);
}
