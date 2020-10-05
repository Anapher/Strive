using Autofac;
using PaderConference.Hubs;
using PaderConference.Hubs.Media;

namespace PaderConference
{
    public class PresentationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RtcMediaConnectionFactory>().As<IRtcMediaConnectionFactory>().SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IConferenceServiceManager<>))
                .AsImplementedInterfaces().SingleInstance();
        }
    }
}