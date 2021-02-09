using Autofac;
using PaderConference.Hubs;
using PaderConference.Presenters;

namespace PaderConference
{
    public class PresentationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ConferenceLinkPresenter>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<CoreHubConnections>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
