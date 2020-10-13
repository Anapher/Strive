using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.Infrastructure.Services.Chat.Filters;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;
using Timer = System.Timers.Timer;

namespace PaderConference.Infrastructure.Services.Chat
{
    public class ChatService : ConferenceService
    {
        private readonly string _conferenceId;

        private readonly Dictionary<string, DateTimeOffset> _currentlyTyping = new Dictionary<string, DateTimeOffset>();
        private readonly object _currentlyTypingLock = new object();

        private readonly ILogger<ChatService> _logger;
        private readonly IMapper _mapper;
        private readonly Queue<ChatMessage> _messages = new Queue<ChatMessage>();
        private readonly ReaderWriterLock _messagesLock = new ReaderWriterLock();
        private readonly ChatOptions _options;
        private readonly IPermissionsService _permissionsService;
        private readonly Timer _refreshUsersTypingTimer;
        private readonly object _refreshUsersTypingTimerLock = new object();
        private readonly ISynchronizedObject<ChatSynchronizedObject> _synchronizedObject;

        private int _messageIdCounter = 1;

        public ChatService(string conferenceId, IMapper mapper, IPermissionsService permissionsService,
            ISynchronizationManager synchronizationManager, IOptions<ChatOptions> options, ILogger<ChatService> logger)
        {
            _conferenceId = conferenceId;
            _mapper = mapper;
            _permissionsService = permissionsService;
            _options = options.Value;
            _logger = logger;

            _synchronizedObject =
                synchronizationManager.Register("chatInfo", new ChatSynchronizedObject(ImmutableList<string>.Empty));
            _refreshUsersTypingTimer = new Timer();
            _refreshUsersTypingTimer.Elapsed += OnRefreshUsersTyping;
            _refreshUsersTypingTimer.Interval = _options.CancelParticipantIsTypingInterval * 1000;
        }

        public override async ValueTask OnClientDisconnected(Participant participant, string connectionId)
        {
            using (_logger.BeginScope("OnClientDisconnected()"))
            using (_logger.BeginScope(new Dictionary<string, object>
                {{"connectionId", connectionId}, {"participantId", participant.ParticipantId}}))
            {
                lock (_currentlyTypingLock)
                {
                    if (!_currentlyTyping.Remove(participant.ParticipantId)) return;
                }

                await UpdateUsersTyping();
            }
        }

        public async ValueTask SetUserIsTyping(IServiceMessage<bool> serviceMessage)
        {
            using (_logger.BeginScope("SendMessage()"))
            using (_logger.BeginScope(serviceMessage.GetScopeData()))
            {
                _logger.LogDebug("is typing: {typing}", serviceMessage.Payload);

                lock (_currentlyTypingLock)
                {
                    if (serviceMessage.Payload)
                    {
                        var now = DateTimeOffset.UtcNow;
                        _currentlyTyping[serviceMessage.Participant.ParticipantId] = now;
                        _logger.LogDebug("Update participant currently typing to {time}", now);
                    }
                    else
                    {
                        if (!_currentlyTyping.Remove(serviceMessage.Participant.ParticipantId))
                        {
                            _logger.LogDebug("Participant did not exist in currentlyTyping, return");
                            return;
                        }

                        _logger.LogDebug("Removed participant from currently typing.");
                    }
                }

                await UpdateUsersTyping();
            }
        }

        public async ValueTask SendMessage(IServiceMessage<SendChatMessageDto> message)
        {
            using (_logger.BeginScope("SendMessage()"))
            using (_logger.BeginScope(message.GetScopeData()))
            {
                _logger.LogDebug("Message: {@message}", message.Payload);
                var messageDto = message.Payload;

                if (string.IsNullOrWhiteSpace(messageDto.Message))
                {
                    _logger.LogDebug("Message is empty, return error");
                    await message.ResponseError(ChatError.EmptyMessageNotAllowed);
                    return;
                }

                var permissions = await _permissionsService.GetPermissions(message.Participant);
                if (!permissions.GetPermission(PermissionsList.Chat.CanSendChatMessage))
                {
                    _logger.LogDebug("Permissions to send chat message denied");
                    await message.ResponseError(ChatError.PermissionToSendMessageDenied);
                    return;
                }

                // here would be the point to implement e. g. language filter

                if (messageDto.Filter != null && messageDto.Filter.Exclude == null && messageDto.Filter.Include == null)
                {
                    _logger.LogDebug("Invalid filter, chat message would be send to nobody");
                    await message.ResponseError(ChatError.InvalidFilter);
                    return; // the message would be sent to nobody
                }

                var filter = FilterFactory.CreateFilter(messageDto.Filter, message.Context.ConnectionId);
                if (!permissions.GetPermission(PermissionsList.Chat.CanSendPrivateChatMessage) &&
                    !(filter is AtAllFilter))
                {
                    await message.ResponseError(ChatError.PermissionToSendPrivateMessageDenied);
                    return;
                }

                ChatMessage chatMessage;

                _messagesLock.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    chatMessage =
                        new ChatMessage(_messageIdCounter++, message.Participant.ParticipantId, messageDto.Message,
                            filter,
                            DateTimeOffset.UtcNow);

                    _messages.Enqueue(chatMessage);

                    // limit the amount of saved chat messages
                    if (_messages.Count > _options.MaxChatMessageHistory)
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

        private async ValueTask UpdateUsersTyping()
        {
            using (_logger.BeginScope("UpdateUsersTyping()"))
            {
                IImmutableList<string> usersCurrentlyTyping;

                lock (_currentlyTypingLock)
                {
                    // query all users that are currently typing and order them
                    usersCurrentlyTyping = _currentlyTyping.Keys.OrderBy(x => x).ToImmutableList();
                }

                _logger.LogDebug("New users currently typing: {@users}", usersCurrentlyTyping);

                // if the list changed, push an update
                if (!usersCurrentlyTyping.SequenceEqual(_synchronizedObject.Current.ParticipantsTyping))
                {
                    _logger.LogDebug("Users currently typing changed, update synchronized object");
                    await _synchronizedObject.Update(new ChatSynchronizedObject(usersCurrentlyTyping));
                }

                lock (_refreshUsersTypingTimerLock)
                {
                    if (usersCurrentlyTyping.Any()) _refreshUsersTypingTimer.Start();
                    else _refreshUsersTypingTimer.Stop();
                }
            }
        }

        private void OnRefreshUsersTyping(object sender, ElapsedEventArgs e)
        {
            using (_logger.BeginScope("OnRefreshUsersTyping()"))
            {
                _logger.LogDebug("Update users currently typing, timeout is {timeout}",
                    _options.CancelParticipantIsTypingAfter);

                var now = DateTimeOffset.UtcNow.AddSeconds(-_options.CancelParticipantIsTypingAfter);

                lock (_currentlyTypingLock)
                {
                    foreach (var (id, dateTimeOffset) in _currentlyTyping)
                        if (now > dateTimeOffset)
                            _currentlyTyping.Remove(id);
                }

                UpdateUsersTyping().Forget();
            }
        }
    }
}