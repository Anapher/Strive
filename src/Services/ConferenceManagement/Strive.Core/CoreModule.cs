using Autofac;
using Autofac.Features.Variance;
using FluentValidation;
using Strive.Core.Services;
using Strive.Core.Services.Chat;
using Strive.Core.Services.ConferenceControl;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers.TalkingStick;
using Strive.Core.Services.Synchronization;

namespace Strive.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
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
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<ISceneProvider>().As<ISceneProvider>()
                .SingleInstance();

            builder.RegisterType<TalkingStickModeHandler>().AsImplementedInterfaces().InstancePerDependency();
        }
    }
}
