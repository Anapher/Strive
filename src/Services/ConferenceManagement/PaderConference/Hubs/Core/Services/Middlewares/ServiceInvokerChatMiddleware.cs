using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Core.Services.Permissions;
using PaderConference.Hubs.Core.Dtos;

namespace PaderConference.Hubs.Core.Services.Middlewares
{
    public static class ServiceInvokerChatMiddleware
    {
        public static IServiceRequestBuilder<TResponse> VerifyCanSendToChatChannel<TResponse>(
            this IServiceRequestBuilder<TResponse> builder, ChatChannel channel)
        {
            return builder.AddMiddleware(context => VerifyCanSendToChatChannel(context, channel));
        }

        public static async ValueTask<SuccessOrError<Unit>> VerifyCanSendToChatChannel(ServiceInvokerContext context,
            ChatChannel channel)
        {
            var selector = context.Context.Resolve<IChatChannelSelector>();
            var canSend = await selector.CanParticipantSendMessageToChannel(context.Participant, channel);

            if (!canSend) return ChatError.InvalidChannel;

            if (channel is PrivateChatChannel)
            {
                var mediator = context.Context.Resolve<IMediator>();
                var conference = await mediator.Send(new FindConferenceByIdRequest(context.Participant.ConferenceId));
                var options = conference.Configuration.Chat;

                if (!options.IsPrivateChatEnabled)
                    return ChatError.PrivateMessagesDisabled;
            }

            return SuccessOrError<Unit>.Succeeded(Unit.Value);
        }

        public static IServiceRequestBuilder<TResponse> VerifyMessageConformsOptions<TResponse>(
            this IServiceRequestBuilder<TResponse> builder, SendChatMessageDto message)
        {
            var neededPermissions = GetRequiredPermissions(message);
            return builder.RequirePermissions(neededPermissions);
        }

        public static IEnumerable<PermissionDescriptor<bool>> GetRequiredPermissions(SendChatMessageDto message)
        {
            yield return DefinedPermissions.Chat.CanSendChatMessage;

            if (message.Options.IsAnonymous)
                yield return DefinedPermissions.Chat.CanSendAnonymously;

            if (message.Options.IsAnnouncement)
                yield return DefinedPermissions.Chat.CanSendAnnouncement;
        }
    }
}
