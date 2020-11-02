using Autofac;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using PaderConference.Infrastructure.Auth;
using PaderConference.Infrastructure.Conferencing;
using PaderConference.Infrastructure.Data;
using PaderConference.Infrastructure.Hubs;
using PaderConference.Infrastructure.Interfaces;
using PaderConference.Infrastructure.Redis;
using PaderConference.Infrastructure.ServiceFactories.Base;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance();
            builder.RegisterType<JwtHandler>().As<IJwtHandler>().SingleInstance();
            builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
            builder.RegisterType<JwtValidator>().As<IJwtValidator>().SingleInstance();

            builder.RegisterType<ConferenceManager>().As<IConferenceManager>().SingleInstance();
            builder.RegisterType<ConnectionMapping>().As<IConnectionMapping>().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IConferenceServiceManager<>))
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IConferenceService>().AsSelf()
                .InstancePerDependency();

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IRedisRepo>().AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<SignalrMessenger<CoreHub>>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(MongoRepo<>)).AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
