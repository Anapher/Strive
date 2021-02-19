using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.Variance;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.ConferenceControl.Gateways;
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

        protected async ValueTask SetParticipantJoined(string conferenceId, string participantId)
        {
            var joinedParticipantRepo = Container.Resolve<IJoinedParticipantsRepository>();
            await joinedParticipantRepo.AddParticipant(participantId, conferenceId, "testConn");
        }

        protected async ValueTask RemoveParticipantJoined(string participantId)
        {
            var joinedParticipantRepo = Container.Resolve<IJoinedParticipantsRepository>();
            await joinedParticipantRepo.RemoveParticipant(participantId, "testConn");
        }
    }
}
