using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.Extensions.Options;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Permissions;
using PaderConference.Hubs.Dtos;

namespace PaderConference.Hubs.Services.Middlewares
{
    public static class ServiceInvokerChatMiddleware
    {
        public static IServiceRequestBuilder<TResponse> VerifyCanSendToChatChannel<TResponse>(
            this IServiceRequestBuilder<TResponse> builder, ChatChannel channel)
        {
            return builder.RequirePermissions(DefinedPermissions.Chat.CanSendChatMessage)
                .AddMiddleware(context => VerifyCanSendToChatChannel(context, channel));
        }

        public static async ValueTask<SuccessOrError<Unit>> VerifyCanSendToChatChannel(ServiceInvokerContext context,
            ChatChannel channel)
        {
            var selector = context.Context.Resolve<IChatChannelSelector>();
            var canSend = await selector.CanParticipantSendMessageToChannel(context.Participant, channel);

            if (!canSend) return ChatError.InvalidChannel;

            return SuccessOrError<Unit>.Succeeded(Unit.Value);
        }

        public static IServiceRequestBuilder<TResponse> VerifyMessageConformsOptions<TResponse>(
            this IServiceRequestBuilder<TResponse> builder, SendChatMessageDto message, ChatChannel chatChannel)
        {
            return builder.AddMiddleware(context =>
                new ValueTask<SuccessOrError<Unit>>(VerifyMessageConformsOptions(context, message, chatChannel)));
        }

        public static SuccessOrError<Unit> VerifyMessageConformsOptions(ServiceInvokerContext context,
            SendChatMessageDto message, ChatChannel channel)
        {
            var options = context.Context.Resolve<IOptions<ChatOptions>>().Value;
            if (!options.CanSendAnonymousMessage && message.Options.IsAnonymous)
                return ChatError.AnonymousMessagesDisabled;

            if (!options.IsPrivateChatEnabled && channel is PrivateChatChannel)
                return ChatError.PrivateMessagesDisabled;

            return SuccessOrError<Unit>.Succeeded(Unit.Value);
        }
    }
}
