using PaderConference.Messaging.SFU.Contracts;
using PaderConference.Messaging.SFU.Dto;

namespace PaderConference.Messaging.SFU.SendContracts
{
    public interface MediaStateChanged : SfuMessage<SfuConferenceInfoUpdate>
    {
    }
}
