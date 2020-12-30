using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services.Chat
{
    public class ChatError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error InvalidMode => BadRequest("Invalid chat message mode.", ServiceErrorCode.Chat_InvalidMode);
    }
}