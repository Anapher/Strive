using System.Threading.Tasks;
using Autofac;
using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Permissions;

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
    }
}
