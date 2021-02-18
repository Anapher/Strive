using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Hubs.Services;
using PaderConference.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Tests.Hubs.Services
{
    public class ServiceRequestBuilderTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<IMediator> _mediator = new();

        public ServiceRequestBuilderTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private IServiceRequestBuilder<TResponse> CreateBuilder<TResponse>(Func<IRequest<TResponse>> requestFactory,
            CancellationToken token = default)
        {
            var container = BuildContainerWithLogger();

            return new ServiceRequestBuilder<TResponse>(requestFactory, _mediator.Object,
                new ServiceInvokerContext(new TestHub(token), container, string.Empty, string.Empty));
        }

        private IServiceRequestBuilder<TResponse> CreateBuilder<TResponse>(
            Func<IRequest<SuccessOrError<TResponse>>> requestFactory, CancellationToken token = default)
        {
            var container = BuildContainerWithLogger();

            return new ServiceRequestBuilderSuccessOrError<TResponse>(requestFactory, _mediator.Object,
                new ServiceInvokerContext(new TestHub(token), container, string.Empty, string.Empty));
        }

        private ILifetimeScope BuildContainerWithLogger()
        {
            var builder = new ContainerBuilder();

            var loggerFactory = _testOutputHelper.CreateLoggerFactory();
            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            return builder.Build();
        }

        [Fact]
        public async Task Send_SimpleResponse_SendWithMediator()
        {
            // arrange
            var builder = CreateBuilder(() => new TestRequest());

            // act
            await builder.Send();

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Send_SimpleResponse_ReturnResult()
        {
            var testResponse = new TestResponse("test");

            // arrange
            _mediator.Setup(x => x.Send(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(testResponse);
            var builder = CreateBuilder(() => new TestRequest());

            // act
            var result = await builder.Send();

            // assert
            Assert.True(result.Success);
            Assert.Same(testResponse, result.Response);
        }

        [Fact]
        public async Task Send_SuccessOrErrorResponse_DontNestSuccessOrError()
        {
            var testResponse = new TestResponse("test");

            // arrange
            _mediator.Setup(x => x.Send(It.IsAny<TestSuccessOrErrorRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessOrError<TestResponse>(testResponse));
            var builder = CreateBuilder(() => new TestSuccessOrErrorRequest());

            // act
            var result = await builder.Send();

            // assert
            Assert.True(result.Success);
            Assert.Same(testResponse, result.Response);
        }

        [Fact]
        public async Task Send_SendErrorOccurred_WrapSendError()
        {
            // arrange
            _mediator.Setup(x => x.Send(It.IsAny<TestSuccessOrErrorRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test"));
            var builder = CreateBuilder(() => new TestSuccessOrErrorRequest());

            // act
            var result = await builder.Send();

            // assert
            Assert.False(result.Success);
            Assert.Equal(result.Error.Code, ServiceErrorCode.Conference_UnexpectedError.ToString());
        }

        [Fact]
        public async Task Send_AddMiddleware_ExecuteMiddlewareAndSendAfterwards()
        {
            // arrange
            var builder = CreateBuilder(() => new TestRequest());

            var middleware = new Mock<ServiceMiddleware>();
            middleware.Setup(x => x(It.IsAny<ServiceInvokerContext>()))
                .ReturnsAsync(SuccessOrError<Unit>.Succeeded(Unit.Value));

            builder.AddMiddleware(middleware.Object);

            // act
            await builder.Send();

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            middleware.Verify(x => x(It.IsAny<ServiceInvokerContext>()), Times.Once);
        }

        [Fact]
        public async Task Send_MiddlewareHasError_DontSendRequestAndFail()
        {
            // arrange
            var builder = CreateBuilder(() => new TestRequest());

            var middleware = new Mock<ServiceMiddleware>();
            middleware.Setup(x => x(It.IsAny<ServiceInvokerContext>()))
                .ReturnsAsync(SuccessOrError<Unit>.Failed(CommonError.ConcurrencyError));

            builder.AddMiddleware(middleware.Object);

            // act
            var result = await builder.Send();

            // assert
            Assert.False(result.Success);
            Assert.Equal(CommonError.ConcurrencyError.Code, result.Error.Code);

            _mediator.Verify(x => x.Send(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        public class TestRequest : IRequest<TestResponse>
        {
        }

        public class TestSuccessOrErrorRequest : IRequest<SuccessOrError<TestResponse>>
        {
        }

        public record TestResponse(string Test);

        public class TestHub : Hub
        {
            public TestHub(CancellationToken cancellationToken)
            {
                var context = new Mock<HubCallerContext>();
                context.SetupGet(x => x.ConnectionAborted).Returns(cancellationToken);
                Context = context.Object;
            }
        }
    }
}
