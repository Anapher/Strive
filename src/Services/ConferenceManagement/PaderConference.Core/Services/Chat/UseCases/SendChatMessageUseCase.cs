using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.Chat.Notifications;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.ParticipantsList.Requests;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Chat.UseCases
{
    public class SendChatMessageUseCase : IRequestHandler<SendChatMessageRequest, SuccessOrError<Unit>>
    {
        private readonly IMediator _mediator;
        private readonly IChatRepository _chatRepository;

        public SendChatMessageUseCase(IMediator mediator, IChatRepository chatRepository)
        {
            _mediator = mediator;
            _chatRepository = chatRepository;
        }

        public async Task<SuccessOrError<Unit>> Handle(SendChatMessageRequest request,
            CancellationToken cancellationToken)
        {
            var (participant, _, channel, _) = request;
            var conferenceId = participant.ConferenceId;

            var message = await BuildChatMessage(request);
            var channelId = ChannelSerializer.Encode(channel);

            var messagesCount =
                await _chatRepository.AddChatMessageAndGetMessageCount(conferenceId, channelId.ToString(), message);

            var subscribedParticipants =
                await _mediator.Send(new FetchSubscribedParticipantsRequest(conferenceId, channelId));

            if (channel is PrivateChatChannel privateChatChannel)
                subscribedParticipants = await UpdateParticipantsSubscriptionsIfNecessary(subscribedParticipants,
                    privateChatChannel, participant.ConferenceId);

            await _mediator.Publish(new ChatMessageReceivedNotification(conferenceId, subscribedParticipants, message,
                messagesCount));

            return Unit.Value;
        }

        private async ValueTask<ChatMessage> BuildChatMessage(SendChatMessageRequest request)
        {
            var timestamp = DateTimeOffset.UtcNow;

            var (participant, messageText, _, options) = request;
            var metadata = await _mediator.Send(new FetchParticipantsMetadataRequest(participant));
            var sender = new ChatMessageSender(participant.Id, metadata);

            return new ChatMessage(sender, messageText, timestamp, options);
        }

        private async ValueTask<List<Participant>> UpdateParticipantsSubscriptionsIfNecessary(
            IReadOnlyList<Participant> subscribedParticipants, PrivateChatChannel channel, string conferenceId)
        {
            var result = subscribedParticipants.ToList();
            foreach (var participantId in channel.Participants)
            {
                if (subscribedParticipants.All(participant => participant.Id != participantId))
                {
                    await _mediator.Send(new UpdateSubscriptionsRequest(new Participant(conferenceId, participantId)));
                    result.Add(new Participant(conferenceId, participantId));
                }
            }

            return result;
        }
    }
}
