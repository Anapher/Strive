using Autofac;
using Strive.Core.Interfaces.Services;
using Strive.Core.Services.ConferenceManagement.Gateways;
using Strive.Infrastructure.Auth;
using Strive.Infrastructure.Data;
using Strive.Infrastructure.Data.Repos;
using Strive.Infrastructure.KeyValue;
using Strive.Infrastructure.Scheduler;
using Strive.Infrastructure.Sfu;

namespace Strive.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IKeyValueRepo>().AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(MongoRepo<>)).AsSelf()
                .AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterType<CachedConferenceRepo>().As<IConferenceRepo>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduledMediator>().As<IScheduledMediator>();
            builder.RegisterType<JwtSfuAuthTokenFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TokenFactory>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
