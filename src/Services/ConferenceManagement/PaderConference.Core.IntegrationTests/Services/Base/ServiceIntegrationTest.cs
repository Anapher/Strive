using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Tests._TestUtils;
using PaderConference.Infrastructure;
using PaderConference.Infrastructure.Redis;
using PaderConference.Infrastructure.Redis.InMemory;
using Xunit.Abstractions;

namespace PaderConference.Core.IntegrationTests.Services.Base
{
    public abstract class ServiceIntegrationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        protected InMemoryKeyValueData Data = new();
        protected ILifetimeScope Container;

        protected IMediator Mediator;

        public ServiceIntegrationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            Container = BuildContainer();
            Mediator = Container.Resolve<IMediator>();
        }

        protected virtual ILifetimeScope BuildContainer()
        {
            var loggerFactory = _testOutputHelper.CreateLoggerFactory();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(Data).AsSelf();
            builder.RegisterType<InMemoryKeyValueDatabase>().AsImplementedInterfaces();

            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            builder.RegisterModule<CoreModule>();

            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            var serviceTypes = FetchServiceTypes().ToArray();
            builder.RegisterTypes(serviceTypes).AsImplementedInterfaces(); // via assembly scan

            builder.RegisterAssemblyTypes(typeof(InfrastructureModule).Assembly).AssignableTo<IRedisRepo>()
                .AsImplementedInterfaces().SingleInstance();

            return builder.Build();
        }

        protected abstract IEnumerable<Type> FetchServiceTypes();

        protected IEnumerable<Type> FetchTypesOfNamespace(Type type)
        {
            return type.Assembly.GetTypes().Where(x => x.IsClass && x.IsInNamespace(type.Namespace));
        }
    }
}
