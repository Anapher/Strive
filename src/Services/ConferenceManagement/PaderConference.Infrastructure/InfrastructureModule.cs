using Autofac;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Infrastructure.Data;
using PaderConference.Infrastructure.Data.Repos;
using PaderConference.Infrastructure.Redis;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Impl;

namespace PaderConference.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IRedisRepo>().AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<RedisKeyValueDatabase>().As<IKeyValueDatabase>().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(MongoRepo<>)).AsSelf()
                .AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterType<CachedConferenceRepo>().As<IConferenceRepo>().InstancePerLifetimeScope();
        }
    }
}
