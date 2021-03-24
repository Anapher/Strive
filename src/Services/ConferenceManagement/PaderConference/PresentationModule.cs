using Autofac;
using PaderConference.Hubs.Core;
using PaderConference.Messaging.SFU;
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
            builder.RegisterType<SfuConferenceInfoProvider>().AsImplementedInterfaces();
            builder.RegisterType<SfuNotifier>().AsImplementedInterfaces();
        }
    }
}
