using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.Variance;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.IntegrationTests._TestHelpers;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceControl.ClientControl;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Infrastructure;
using PaderConference.Infrastructure.Redis;
using PaderConference.Infrastructure.Redis.InMemory;
using PaderConference.Tests.Utils;
using Xunit.Abstractions;

namespace PaderConference.Core.IntegrationTests.Services.Base
{
    public abstract class ServiceIntegrationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        protected InMemoryKeyValueData Data = new();
        protected ILifetimeScope Container;

        protected IMediator Mediator;

        protected MediatorNotificationCollector NotificationCollector = new();
        protected readonly SynchronizedObjectListener SynchronizedObjectListener = new();

        protected ServiceIntegrationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            Container = BuildContainer();
            Mediator = Container.Resolve<IMediator>();

            SetupTest(Container).Wait();
        }

        protected virtual Task SetupTest(ILifetimeScope container)
        {
            return Task.CompletedTask;
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterInstance(NotificationCollector).AsImplementedInterfaces();
            builder.RegisterInstance(SynchronizedObjectListener).AsImplementedInterfaces();

            builder.RegisterInstance(Data).AsSelf();
            builder.RegisterType<InMemoryKeyValueDatabase>().AsImplementedInterfaces();

            var loggerFactory = _testOutputHelper.CreateLoggerFactory();
            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            //builder.RegisterModule<CoreModule>();

            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            var serviceTypes = FetchServiceTypes().ToArray();
            builder.RegisterTypes(serviceTypes).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(InfrastructureModule).Assembly).AssignableTo<IRedisRepo>()
                .AsImplementedInterfaces().SingleInstance();
        }

        protected virtual ILifetimeScope BuildContainer()
        {
            var builder = new ContainerBuilder();
            ConfigureContainer(builder);

            return builder.Build();
        }

        protected abstract IEnumerable<Type> FetchServiceTypes();

        protected void SetupConferenceControl(ContainerBuilder builder, Func<Type, bool>? typeFilter = null)
        {
            builder.RegisterInstance(new OptionsWrapper<ConferenceSchedulerOptions>(new ConferenceSchedulerOptions()))
                .AsImplementedInterfaces();

            var types = FetchTypesOfNamespace(typeof(SynchronizedConferenceInfo));
            if (typeFilter != null) types = types.Where(typeFilter);
            builder.RegisterTypes(types.ToArray()).AsImplementedInterfaces();

            var mockMessagingHandler = new Mock<IRequestHandler<EnableParticipantMessagingRequest>>();
            builder.RegisterInstance(mockMessagingHandler.Object).AsImplementedInterfaces();
        }

        protected IEnumerable<Type> FetchTypesOfNamespace(Type type)
        {
            return type.Assembly.GetTypes().Where(x => x.IsClass && x.IsInNamespace(type.Namespace!));
        }

        protected IEnumerable<Type> FetchTypesForSynchronizedObjects()
        {
            return FetchTypesOfNamespace(typeof(SynchronizedObjectId));
        }

        protected void AddConferenceRepo(ContainerBuilder builder, Conference conference)
        {
            var mock = new Mock<IConferenceRepo>();
            mock.Setup(x => x.FindById(conference.ConferenceId)).ReturnsAsync(conference);

            builder.RegisterInstance(mock.Object).As<IConferenceRepo>();
        }
    }
}
