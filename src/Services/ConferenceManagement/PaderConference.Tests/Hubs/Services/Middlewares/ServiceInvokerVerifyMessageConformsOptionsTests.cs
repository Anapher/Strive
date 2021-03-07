using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Hubs.Dtos;
using PaderConference.Hubs.Services;
using PaderConference.Hubs.Services.Middlewares;
using Xunit;

namespace PaderConference.Tests.Hubs.Services.Middlewares
{
    public class ServiceInvokerVerifyMessageConformsOptionsTests : MiddlewareTestBase
    {
        protected override IServiceRequestBuilder<string> Execute(IServiceRequestBuilder<string> builder)
        {
            return builder.VerifyMessageConformsOptions(
                new SendChatMessageDto("test", "test", new ChatMessageOptions()), new GlobalChatChannel());
        }

        private IMediator SetupMediatorWithConference(Conference conference)
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<FindConferenceByIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(conference);

            return mediatorMock.Object;
        }

        [Fact]
        public async Task VerifyMessageConformsOptions_AnonymousMessagesDisabled_ReturnError()
        {
            // arrange
            var mediator =
                SetupMediatorWithConference(new Conference("123")
                {
                    Configuration = {Chat = {CanSendAnonymousMessage = false}},
                });

            var messageDto = new SendChatMessageDto("Hello", "test", new ChatMessageOptions {IsAnonymous = true});

            var context = CreateContext(builder => builder.RegisterInstance(mediator).AsImplementedInterfaces());

            // act
            var result = await ServiceInvokerChatMiddleware.VerifyMessageConformsOptions(context, messageDto,
                GlobalChatChannel.Instance);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ChatError.AnonymousMessagesDisabled.Code, result.Error!.Code);
        }

        [Fact]
        public async Task VerifyMessageConformsOptions_PrivateChatDisabled_ReturnError()
        {
            // arrange
            var mediator =
                SetupMediatorWithConference(new Conference("123")
                {
                    Configuration = {Chat = {IsPrivateChatEnabled = false}},
                });

            var messageDto = new SendChatMessageDto("Hello", "test", new ChatMessageOptions());
            var context = CreateContext(builder => builder.RegisterInstance(mediator).AsImplementedInterfaces());
            var channel = new PrivateChatChannel(new HashSet<string> {"asd", "asd2"});

            // act
            var result = await ServiceInvokerChatMiddleware.VerifyMessageConformsOptions(context, messageDto, channel);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ChatError.PrivateMessagesDisabled.Code, result.Error!.Code);
        }

        [Fact]
        public async Task VerifyMessageConformsOptions_EverythingOk_ReturnSuccess()
        {
            // arrange
            var mediator = SetupMediatorWithConference(new Conference("123"));

            var messageDto = new SendChatMessageDto("Hello", "test", new ChatMessageOptions());
            var context = CreateContext(builder => builder.RegisterInstance(mediator).AsImplementedInterfaces());

            // act
            var result =
                await ServiceInvokerChatMiddleware.VerifyMessageConformsOptions(context, messageDto,
                    GlobalChatChannel.Instance);

            // assert
            Assert.True(result.Success);
        }
    }
}
