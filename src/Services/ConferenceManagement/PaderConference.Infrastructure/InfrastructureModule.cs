using Autofac;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Data;
using PaderConference.Infrastructure.Data.Repos;
using PaderConference.Infrastructure.Redis;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConnectionMapping>().As<IConnectionMapping>().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IRedisRepo>().AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(MongoRepo<>)).AsSelf()
                .AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterType<CachedConferenceRepo>().As<IConferenceRepo>().InstancePerDependency();
        }
    }
}
