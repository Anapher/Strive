using Autofac;
using Strive.Hubs.Core;
using Strive.Messaging.SFU;
using Strive.Presenters;

namespace Strive
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
