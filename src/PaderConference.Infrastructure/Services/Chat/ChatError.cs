namespace PaderConference.Infrastructure.Services.Chat
{
    public class ChatError : ServiceError
    {
        public ChatError(string message, ServiceErrorCode code) : base(message, code)
        {
        }

        public static ChatError EmptyMessageNotAllowed =>
            new ChatError("An empty message is not allowed.", ServiceErrorCode.Chat_EmptyMessage);

        public static ChatError PermissionToSendMessageDenied =>
            new ChatError("Permissions to send a chat message denied.",
                ServiceErrorCode.Chat_PermissionDenied_SendMessage);

        public static ChatError PermissionToSendAnonymousMessageDenied =>
            new ChatError("Permissions to send this message as anonymous denied.",
                ServiceErrorCode.Chat_PermissionDenied_SendAnonymousMessage);

        public static ChatError PermissionToSendPrivateMessageDenied =>
            new ChatError("Permissions to send private messages denied.",
                ServiceErrorCode.Chat_PermissionDenied_SendPrivateMessage);

        public static ChatError InvalidFilter =>
            new ChatError("Invalid chat message filter.", ServiceErrorCode.Chat_InvalidFilter);
    }
}