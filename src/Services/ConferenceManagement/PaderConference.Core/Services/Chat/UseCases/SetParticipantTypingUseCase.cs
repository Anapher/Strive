using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Chat.UseCases
{
    public class SetParticipantTypingUseCase : IRequestHandler<SetParticipantTypingRequest>
    {
        private readonly IMediator _mediator;
        private readonly IChatRepository _chatRepository;
        private readonly IParticipantTypingTimer _participantTypingTimer;
        private readonly ChatOptions _options;

        public SetParticipantTypingUseCase(IMediator mediator, IChatRepository chatRepository,
            IParticipantTypingTimer participantTypingTimer, IOptions<ChatOptions> options)
        {
            _mediator = mediator;
            _chatRepository = chatRepository;
            _participantTypingTimer = participantTypingTimer;
            _options = options.Value;
        }

        public async Task<Unit> Handle(SetParticipantTypingRequest request, CancellationToken cancellationToken)
        {
            var (participant, channel, isTyping) = request;

            if (isTyping)
            {
                var added = await _chatRepository.AddParticipantTyping(participant, channel);
                _participantTypingTimer.RemoveParticipantTypingAfter(participant, channel,
                    TimeSpan.FromSeconds(_options.CancelParticipantIsTypingAfter));

                if (!added) return Unit.Value;
            }
            else
            {
                var removed = await _chatRepository.RemoveParticipantTyping(participant, channel);
                if (removed)
                    _participantTypingTimer.CancelTimer(participant, channel);
                else return Unit.Value;
            }

            if (_options.ShowTyping)
            {
                var syncObjId = SynchronizedObjectId.Parse(channel);
                await _mediator.Send(new UpdateSynchronizedObjectRequest(participant.ConferenceId, syncObjId));
            }

            return Unit.Value;
        }
    }
}
