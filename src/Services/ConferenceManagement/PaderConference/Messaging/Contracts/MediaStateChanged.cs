using PaderConference.Messaging.SFU.Dto;

namespace PaderConference.Messaging.Contracts
{
    public interface MediaStateChanged
    {
        string ConferenceId { get; }

        SfuConferenceInfoUpdate Update { get; }
    }
}
