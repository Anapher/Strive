using System;
using Autofac;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PaderConference.Core.Services;
using PaderConference.Hubs.Services;
using Xunit;

namespace PaderConference.Tests.Hubs.Services.Middlewares
{
    public abstract class MiddlewareTestBase
    {
        protected const string ConferenceId = "test";
        protected const string ParticipantId = "test2";

        private static readonly Participant TestParticipant = new(ConferenceId, ParticipantId);

        protected ServiceInvokerContext CreateContext(Action<ContainerBuilder>? configureContainer = null)
        {
            var builder = new ContainerBuilder();
            configureContainer?.Invoke(builder);

            return new ServiceInvokerContext(new Mock<Hub>().Object, builder.Build(), TestParticipant);
        }

        protected abstract IServiceRequestBuilder<string> Execute(IServiceRequestBuilder<string> builder);

        [Fact]
        public void OnExecution_AddsMiddleware()
        {
            // arrange
            var builder = new Mock<IServiceRequestBuilder<string>>();

            // act
            Execute(builder.Object);

            // assert
            builder.Verify(x => x.AddMiddleware(It.IsAny<ServiceMiddleware>()), Times.Once);
        }

        [Fact]
        public void OnExecution_ReturnsSameBuilderInstance()
        {
            // arrange
            var builder = new Mock<IServiceRequestBuilder<string>>();
            builder.Setup(x => x.AddMiddleware(It.IsAny<ServiceMiddleware>())).Returns(builder.Object);

            // act
            var result = Execute(builder.Object);

            // assert
            Assert.Same(builder.Object, result);
        }
    }
}