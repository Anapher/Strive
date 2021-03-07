using System.Threading.Tasks;
using Autofac;
using Moq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Hubs.Services;
using PaderConference.Hubs.Services.Middlewares;
using Xunit;

namespace PaderConference.Tests.Hubs.Services.Middlewares
{
    public class ServiceInvokerChatVerifyCanSendToChatChannelTests : MiddlewareTestBase
    {
        protected override IServiceRequestBuilder<string> Execute(IServiceRequestBuilder<string> builder)
        {
            return builder.VerifyCanSendToChatChannel(GlobalChatChannel.Instance);
        }

        [Fact]
        public async Task VerifyCanSendToChatChannel_CannotSend_ReturnError()
        {
            // arrange
            var channelSelector = new Mock<IChatChannelSelector>();
            channelSelector
                .Setup(x => x.CanParticipantSendMessageToChannel(It.IsAny<Participant>(), It.IsAny<ChatChannel>()))
                .ReturnsAsync(false);

            var context = CreateContext(builder =>
                builder.RegisterInstance(channelSelector.Object).AsImplementedInterfaces());

            // act
            var result =
                await ServiceInvokerChatMiddleware.VerifyCanSendToChatChannel(context, GlobalChatChannel.Instance);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ChatError.InvalidChannel.Code, result.Error!.Code);
        }

        [Fact]
        public async Task VerifyCanSendToChatChannel_CanSend_ReturnSuccess()
        {
            // arrange
            var channelSelector = new Mock<IChatChannelSelector>();
            channelSelector
                .Setup(x => x.CanParticipantSendMessageToChannel(It.IsAny<Participant>(), It.IsAny<ChatChannel>()))
                .ReturnsAsync(true);

            var context = CreateContext(builder =>
                builder.RegisterInstance(channelSelector.Object).AsImplementedInterfaces());

            // act
            var result =
                await ServiceInvokerChatMiddleware.VerifyCanSendToChatChannel(context, GlobalChatChannel.Instance);

            // assert
            Assert.True(result.Success);
        }
    }
}
