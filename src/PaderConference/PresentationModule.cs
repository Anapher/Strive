using Autofac;
using PaderConference.Hubs.Chat;

namespace PaderConference
{
    public class PresentationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ChatManager>().As<IChatManager>().SingleInstance();
        }
    }
}