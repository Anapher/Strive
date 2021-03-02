using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Chat.UseCases
{
    public class SetParticipantTypingUseCase : IRequestHandler<SetParticipantTypingRequest>
    {
        private readonly IMediator _mediator;
        private readonly IChatRepository _chatRepository;
        private readonly IParticipantTypingTimer _participantTypingTimer;

        public SetParticipantTypingUseCase(IMediator mediator, IChatRepository chatRepository,
            IParticipantTypingTimer participantTypingTimer)
        {
            _mediator = mediator;
            _chatRepository = chatRepository;
            _participantTypingTimer = participantTypingTimer;
        }

        public async Task<Unit> Handle(SetParticipantTypingRequest request, CancellationToken cancellationToken)
        {
            var (participant, channel, isTyping) = request;

            if (isTyping)
                await AddParticipantTyping(participant, channel);
            else
                await RemoveParticipantTyping(participant, channel);

            return Unit.Value;
        }

        private async ValueTask AddParticipantTyping(Participant participant, ChatChannel channel)
        {
            var channelId = GetChannelId(channel).ToString();
            var conference = await _mediator.Send(new FindConferenceByIdRequest(participant.ConferenceId));

            var added = await _chatRepository.AddParticipantTyping(participant, channelId);
            _participantTypingTimer.RemoveParticipantTypingAfter(participant, channel,
                TimeSpan.FromSeconds(conference.Configuration.Chat.CancelParticipantIsTypingAfter));

            if (added) await NotifyParticipantTypingUpdatedIfEnabled(participant.ConferenceId, channel, conference);
        }

        private async ValueTask RemoveParticipantTyping(Participant participant, ChatChannel channel)
        {
            var channelId = GetChannelId(channel).ToString();

            var removed = await _chatRepository.RemoveParticipantTyping(participant, channelId);
            if (removed)
            {
                _participantTypingTimer.CancelTimer(participant, channel);

                var conference = await _mediator.Send(new FindConferenceByIdRequest(participant.ConferenceId));
                await NotifyParticipantTypingUpdatedIfEnabled(participant.ConferenceId, channel, conference);
            }
        }

        private async ValueTask NotifyParticipantTypingUpdatedIfEnabled(string conferenceId, ChatChannel channel,
            Conference conference)
        {
            if (conference.Configuration.Chat.ShowTyping) await NotifyParticipantTypingUpdated(conferenceId, channel);
        }

        private async ValueTask NotifyParticipantTypingUpdated(string conferenceId, ChatChannel channel)
        {
            var channelId = GetChannelId(channel);
            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId, channelId));
        }

        private SynchronizedObjectId GetChannelId(ChatChannel channel)
        {
            return ChannelSerializer.Encode(channel);
        }
    }
}
