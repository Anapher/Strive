using PaderConference.Core.Services.ConferenceControl.Notifications;

namespace PaderConference.Hubs.Responses
{
    public record RequestDisconnectDto(ParticipantKickedReason Reason);
}
