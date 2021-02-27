using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services.Chat
{
    public class ChatError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error InvalidChannel =>
            BadRequest("Cannot send a message to the requested channel.", ServiceErrorCode.Chat_InvalidChannel);
    }
}
