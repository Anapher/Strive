using System.Linq;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.Permissions;
using PaderConference.Hubs.Core.Dtos;
using PaderConference.Hubs.Core.Services;
using PaderConference.Hubs.Core.Services.Middlewares;
using Xunit;

namespace PaderConference.Tests.Hubs.Core.Services.Middlewares
{
    public class ServiceInvokerVerifyMessageConformsOptionsTests : MiddlewareTestBase
    {
        protected override IServiceRequestBuilder<string> Execute(IServiceRequestBuilder<string> builder)
        {
            return builder.VerifyMessageConformsOptions(
                new SendChatMessageDto("test", "test", new ChatMessageOptions()));
        }

        [Fact]
        public void GetRequiredPermissions_NoOptionsConfigured_ReturnCanSendChatMessagePermission()
        {
            // arrange
            var messageDto = new SendChatMessageDto("Hello", "test", new ChatMessageOptions());

            // act
            var requiredPermissions = ServiceInvokerChatMiddleware.GetRequiredPermissions(messageDto);

            // assert
            var permission = Assert.Single(requiredPermissions);
            Assert.Equal(DefinedPermissions.Chat.CanSendChatMessage.Key, permission!.Key);
        }

        [Fact]
        public void GetRequiredPermissions_IsAnonymously_ReturnCanSendChatMessageAndCanSendAnonymouslyPermission()
        {
            // arrange
            var messageDto = new SendChatMessageDto("Hello", "test", new ChatMessageOptions {IsAnonymous = true});

            // act
            var requiredPermissions = ServiceInvokerChatMiddleware.GetRequiredPermissions(messageDto).ToList();

            // assert
            Assert.Equal(2, requiredPermissions.Count);
            Assert.Contains(requiredPermissions, x => x.Key == DefinedPermissions.Chat.CanSendChatMessage.Key);
            Assert.Contains(requiredPermissions, x => x.Key == DefinedPermissions.Chat.CanSendAnonymously.Key);
        }

        [Fact]
        public void GetRequiredPermissions_IsAnnouncement_ReturnCanSendChatMessageAndCanSendAnonymouslyPermission()
        {
            // arrange
            var messageDto = new SendChatMessageDto("Hello", "test", new ChatMessageOptions {IsAnnouncement = true});

            // act
            var requiredPermissions = ServiceInvokerChatMiddleware.GetRequiredPermissions(messageDto).ToList();

            // assert
            Assert.Equal(2, requiredPermissions.Count);
            Assert.Contains(requiredPermissions, x => x.Key == DefinedPermissions.Chat.CanSendChatMessage.Key);
            Assert.Contains(requiredPermissions, x => x.Key == DefinedPermissions.Chat.CanSendAnnouncement.Key);
        }
    }
}
