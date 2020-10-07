using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.Infrastructure.Services.Chat.Filters;

namespace PaderConference.Infrastructure.Services.Chat
{
    public class ChatService : ConferenceService
    {
        private readonly Conference _conference;
        private readonly ILogger<ChatService> _logger;
        private readonly IMapper _mapper;
        private readonly Queue<ChatMessage> _messages = new Queue<ChatMessage>();
        private readonly ReaderWriterLock _messagesLock = new ReaderWriterLock();
        private int _messageIdCounter = 1;

        public ChatService(Conference conference, IMapper mapper,
            ILogger<ChatService> logger)
        {
            _conference = conference;
            _mapper = mapper;
            _logger = logger;
        }

        public async ValueTask SendMessage(IServiceMessage<SendChatMessageDto> message)
        {
            var messageDto = message.Payload;

            if (messageDto.Message == null)
                throw new InvalidOperationException("Null message not allowed");

            // here would be the point to implement e. g. language filter

            if (messageDto.Filter != null && messageDto.Filter.Exclude == null && messageDto.Filter.Include == null)
                return; // the message would be sent to nobody

            var filter = FilterFactory.CreateFilter(messageDto.Filter, message.Context.ConnectionId);
            if (!_conference.Settings.Chat.AllowPrivateConversation && !(filter is AtAllFilter))
                return; // reject

            ChatMessage chatMessage;

            _messagesLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                chatMessage =
                    new ChatMessage(_messageIdCounter++, message.Participant.ParticipantId, messageDto.Message, filter,
                        DateTimeOffset.UtcNow);

                _messages.Enqueue(chatMessage);

                // limit the amount of saved chat messages
                if (_messages.Count > _conference.Settings.Chat.MessageHistoryCount)
                    _messages.Dequeue();
            }
            finally
            {
                _messagesLock.ReleaseWriterLock();
            }

            var dto = _mapper.Map<ChatMessageDto>(chatMessage);
            await chatMessage.Filter.SendTo(message.Clients, message.Participant.Conference.ConferenceId)
                .SendAsync(CoreHubMessages.Response.ChatMessage, dto);
        }

        public async ValueTask RequestAllMessages(IServiceMessage message)
        {
            List<ChatMessage> messages;

            _messagesLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                messages = _messages.Where(x => x.Filter.ShowMessageTo(message.Context.ConnectionId)).ToList(); // copy
            }
            finally
            {
                _messagesLock.ReleaseReaderLock();
            }

            await message.Clients.Caller.SendAsync(CoreHubMessages.Response.Chat,
                messages.Select(_mapper.Map<ChatMessageDto>).ToList());
        }
    }
}