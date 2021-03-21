using PaderConference.Messaging.SFU.Contracts;
using PaderConference.Messaging.SFU.Dto;

namespace PaderConference.Messaging.SFU.ReceiveContracts
{
    public interface SendMessageToConnection : SfuMessage<SendHubMessageDto>
    {
    }
}
