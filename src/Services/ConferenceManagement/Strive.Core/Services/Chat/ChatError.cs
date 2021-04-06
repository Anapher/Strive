using Strive.Core.Dto;
using Strive.Core.Errors;

namespace Strive.Core.Services.Chat
{
    public class ChatError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error InvalidChannel =>
            BadRequest("Cannot send a message to the requested channel.", ServiceErrorCode.Chat_InvalidChannel);

        public static Error AnonymousMessagesDisabled =>
            BadRequest("Cannot send anonymous messages as they are disabled.",
                ServiceErrorCode.Chat_AnonymousMessagesDisabled);

        public static Error PrivateMessagesDisabled =>
            BadRequest("Cannot send private messages as they are disabled.",
                ServiceErrorCode.Chat_PrivateMessagesDisabled);
    }
}
