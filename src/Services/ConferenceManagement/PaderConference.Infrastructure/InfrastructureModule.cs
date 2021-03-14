using Autofac;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.ConferenceManagement.Gateways;
using PaderConference.Infrastructure.Data;
using PaderConference.Infrastructure.Data.Repos;
using PaderConference.Infrastructure.KeyValue;
using PaderConference.Infrastructure.Scheduler;

namespace PaderConference.Infrastructure
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
        }
    }
}
