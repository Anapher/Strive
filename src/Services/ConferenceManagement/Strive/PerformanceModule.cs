using Autofac;
using Strive.Messaging.SFU;

namespace Strive
{
    public class PerformanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<CachedSfuNotifier>().As<ISfuNotifier>().InstancePerLifetimeScope();
        }
    }
}
