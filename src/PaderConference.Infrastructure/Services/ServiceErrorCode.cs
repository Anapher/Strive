// ReSharper disable InconsistentNaming

namespace PaderConference.Infrastructure.Services
{
    public enum ServiceErrorCode
    {
        // Chat
        Chat_EmptyMessage = 1000000,
        Chat_PermissionDenied_SendMessage = 1000001,
        Chat_PermissionDenied_SendAnonymousMessage = 1000002,
        Chat_PermissionDenied_SendPrivateMessage = 1000002,
        Chat_InvalidFilter = 1000003
    }
}