using Autofac;
using PaderConference.Core.Services.Chat;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class ChatServiceManager : AutowiredConferenceServiceManager<ChatService>
    {
        public ChatServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}