using Autofac;
using Autofac.Features.Variance;
using FluentValidation;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // so we can use bases class in mediator
            builder.RegisterSource(new ContravariantRegistrationSource());

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(AbstractValidator<>))
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<ConferenceScheduler>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IPermissionLayerProvider>()
                .As<IPermissionLayerProvider>().InstancePerDependency();
            builder.RegisterType<PermissionLayersAggregator>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ParticipantPermissions>().AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<ISynchronizedObjectProvider>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<DefinedPermissionValidator>().As<IPermissionValidator>().SingleInstance();
            builder.RegisterType<TaskDelay>().As<ITaskDelay>().SingleInstance();
            builder.RegisterType<ChatChannelSelector>().As<IChatChannelSelector>();
            builder.RegisterType<ParticipantTypingTimer>().As<IParticipantTypingTimer>().SingleInstance();
        }
    }
}
