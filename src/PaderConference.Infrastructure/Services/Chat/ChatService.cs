using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.Infrastructure.Services.Chat.Filters;
using PaderConference.Infrastructure.Services.Permissions;

namespace PaderConference.Infrastructure.Services.Chat
{
    public class ChatService : ConferenceService
    {
        private const int MessageHistoryCount = 500;
        private readonly string _conferenceId;
        private readonly ILogger<ChatService> _logger;
        private readonly IMapper _mapper;
        private readonly Queue<ChatMessage> _messages = new Queue<ChatMessage>();
        private readonly ReaderWriterLock _messagesLock = new ReaderWriterLock();
        private readonly IPermissionsService _permissionsService;
        private int _messageIdCounter = 1;

        public ChatService(string conferenceId, IMapper mapper, IPermissionsService permissionsService,
            ILogger<ChatService> logger)
        {
            _conferenceId = conferenceId;
            _mapper = mapper;
            _permissionsService = permissionsService;
            _logger = logger;
        }

        public async ValueTask SendMessage(IServiceMessage<SendChatMessageDto> message)
        {
            var messageDto = message.Payload;

            if (messageDto.Message == null)
                throw new InvalidOperationException("Null message not allowed");

            var permissions = await _permissionsService.GetPermissions(message.Participant);

            // here would be the point to implement e. g. language filter

            if (messageDto.Filter != null && messageDto.Filter.Exclude == null && messageDto.Filter.Include == null)
                return; // the message would be sent to nobody

            var filter = FilterFactory.CreateFilter(messageDto.Filter, message.Context.ConnectionId);
            if (!permissions.CanSendPrivateChatMessages && !(filter is AtAllFilter))
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
                if (_messages.Count > MessageHistoryCount)
                    _messages.Dequeue();
            }
            finally
            {
                _messagesLock.ReleaseWriterLock();
            }

            var dto = _mapper.Map<ChatMessageDto>(chatMessage);
            await chatMessage.Filter.SendTo(message.Clients, _conferenceId)
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