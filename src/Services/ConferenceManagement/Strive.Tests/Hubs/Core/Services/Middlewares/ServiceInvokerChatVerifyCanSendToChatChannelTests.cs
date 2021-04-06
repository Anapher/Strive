using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Moq;
using Strive.Core.Domain.Entities;
using Strive.Core.Services;
using Strive.Core.Services.Chat;
using Strive.Core.Services.Chat.Channels;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Hubs.Core.Services;
using Strive.Hubs.Core.Services.Middlewares;
using Xunit;

namespace Strive.Tests.Hubs.Core.Services.Middlewares
{
    public class ServiceInvokerChatVerifyCanSendToChatChannelTests : MiddlewareTestBase
    {
        protected override IServiceRequestBuilder<string> Execute(IServiceRequestBuilder<string> builder)
        {
            return builder.VerifyCanSendToChatChannel(GlobalChatChannel.Instance);
        }

        private static IChatChannelSelector CreateChatChannelSelector(bool canSend)
        {
            var channelSelector = new Mock<IChatChannelSelector>();
            channelSelector
                .Setup(x => x.CanParticipantSendMessageToChannel(It.IsAny<Participant>(), It.IsAny<ChatChannel>()))
                .ReturnsAsync(canSend);

            return channelSelector.Object;
        }

        private static IMediator CreateMediatorWithFindConference(ChatOptions options)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<FindConferenceByIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Conference("123") {Configuration = new ConferenceConfiguration {Chat = options}});

            return mockMediator.Object;
        }

        [Fact]
        public async Task VerifyCanSendToChatChannel_ChannelSelectorSaysNo_ReturnError()
        {
            // arrange
            var channelSelector = CreateChatChannelSelector(false);

            var context = CreateContext(builder => builder.RegisterInstance(channelSelector).AsImplementedInterfaces());

            // act
            var result =
                await ServiceInvokerChatMiddleware.VerifyCanSendToChatChannel(context, GlobalChatChannel.Instance);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ChatError.InvalidChannel.Code, result.Error!.Code);
        }

        [Fact]
        public async Task VerifyCanSendToChatChannel_PrivateChatDisabled_ReturnError()
        {
            // arrange
            var channelSelector = CreateChatChannelSelector(true);
            var mediator = CreateMediatorWithFindConference(new ChatOptions {IsPrivateChatEnabled = false});

            var context = CreateContext(builder =>
            {
                builder.RegisterInstance(channelSelector).AsImplementedInterfaces();
                builder.RegisterInstance(mediator).AsImplementedInterfaces();
            });

            var channel = new PrivateChatChannel(new HashSet<string> {"1", "2"});

            // act
            var result = await ServiceInvokerChatMiddleware.VerifyCanSendToChatChannel(context, channel);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ChatError.PrivateMessagesDisabled.Code, result.Error!.Code);
        }

        [Fact]
        public async Task VerifyCanSendToChatChannel_CanSend_ReturnSuccess()
        {
            // arrange
            var channelSelector = CreateChatChannelSelector(true);
            var mediator = CreateMediatorWithFindConference(new ChatOptions());

            var context = CreateContext(builder =>
            {
                builder.RegisterInstance(channelSelector).AsImplementedInterfaces();
                builder.RegisterInstance(mediator).AsImplementedInterfaces();
            });

            // act
            var result =
                await ServiceInvokerChatMiddleware.VerifyCanSendToChatChannel(context, GlobalChatChannel.Instance);

            // assert
            Assert.True(result.Success);
        }
    }
}
