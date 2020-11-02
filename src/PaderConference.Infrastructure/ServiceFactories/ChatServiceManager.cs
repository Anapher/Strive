using Autofac;
using PaderConference.Core.Services.Chat;

namespace PaderConference.Infrastructure.Services.Chat
{
    public class ChatServiceManager : AutowiredConferenceServiceManager<ChatService>
    {
        public ChatServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}