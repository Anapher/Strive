using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Hubs.Chat.Filters;
using PaderConference.Infrastructure.Sockets;
using PaderConference.Models.Signal;

namespace PaderConference.Hubs.Chat
{
    public class ConferenceChat
    {
        private readonly Conference _conference;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<ConferenceChat> _logger;
        private readonly IMapper _mapper;
        private readonly Queue<ChatMessage> _messages = new Queue<ChatMessage>();
        private readonly ReaderWriterLock _messagesLock = new ReaderWriterLock();
        private int _messageIdCounter = 1;

        public ConferenceChat(Conference conference, IConnectionMapping connectionMapping, IMapper mapper,
            ILogger<ConferenceChat> logger)
        {
            _conference = conference;
            _connectionMapping = connectionMapping;
            _mapper = mapper;
            _logger = logger;
        }

        public async ValueTask SendMessage(SendChatMessageDto message, HubCallerContext context,
            IHubCallerClients clients)
        {
            if (!_connectionMapping.Connections.TryGetValue(context.ConnectionId, out var participant))
            {
                _logger.LogWarning(
                    "A connection {connectionId} tried to sent a chat message but is not associated with a conference.",
                    context.ConnectionId);
                throw new InvalidOperationException("Not associated to a conference");
            }

            if (message.Message == null)
                throw new InvalidOperationException("Null message not allowed");

            // here would be the point to implement e. g. language filter

            if (message.Filter != null && message.Filter.Exclude == null && message.Filter.Include == null)
                return; // the message would be sent to nobody

            var filter = FilterFactory.CreateFilter(message.Filter, context.ConnectionId);
            if (!_conference.Settings.Chat.AllowPrivateConversation && !(filter is AtAllFilter))
                return; // reject

            ChatMessage chatMessage;

            _messagesLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                chatMessage =
                    new ChatMessage(_messageIdCounter++, participant.ParticipantId, message.Message, filter,
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
            await chatMessage.Filter.SendTo(clients, participant.Conference.ConferenceId)
                .SendAsync(CoreHubMessages.Response.ChatMessage, dto);
        }

        public async ValueTask RequestAllMessages(HubCallerContext context, IHubCallerClients clients)
        {
            List<ChatMessage> messages;

            _messagesLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                messages = _messages.Where(x => x.Filter.ShowMessageTo(context.ConnectionId)).ToList(); // copy
            }
            finally
            {
                _messagesLock.ReleaseReaderLock();
            }

            await clients.Caller.SendAsync(CoreHubMessages.Response.Chat,
                messages.Select(_mapper.Map<ChatMessageDto>).ToList());
        }
    }
}