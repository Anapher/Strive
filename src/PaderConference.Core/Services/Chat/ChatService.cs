using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Chat.Dto;
using PaderConference.Core.Services.Chat.Filters;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Signaling;
using Timer = System.Timers.Timer;

namespace PaderConference.Core.Services.Chat
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
        private readonly IConnectionMapping _connectionMapping;
        private readonly ISignalMessenger _signalMessenger;
        private readonly Timer _refreshUsersTypingTimer;
        private readonly object _refreshUsersTypingTimerLock = new object();
        private readonly ISynchronizedObject<ChatSynchronizedObject> _synchronizedObject;

        private int _messageIdCounter = 1;

        public ChatService(string conferenceId, IMapper mapper, IPermissionsService permissionsService,
            ISynchronizationManager synchronizationManager, IConnectionMapping connectionMapping,
            ISignalMessenger signalMessenger, IOptions<ChatOptions> options, ILogger<ChatService> logger)
        {
            _conferenceId = conferenceId;
            _mapper = mapper;
            _permissionsService = permissionsService;
            _connectionMapping = connectionMapping;
            _signalMessenger = signalMessenger;
            _options = options.Value;
            _logger = logger;

            _synchronizedObject =
                synchronizationManager.Register("chatInfo", new ChatSynchronizedObject(ImmutableList<string>.Empty));
            _refreshUsersTypingTimer = new Timer();
            _refreshUsersTypingTimer.Elapsed += OnRefreshUsersTyping;
            _refreshUsersTypingTimer.Interval = _options.CancelParticipantIsTypingInterval * 1000;
        }

        public override async ValueTask OnClientDisconnected(Participant participant)
        {
            using var _ =
                _logger.BeginMethodScope(new Dictionary<string, object> {{"participantId", participant.ParticipantId}});

            lock (_currentlyTypingLock)
            {
                if (!_currentlyTyping.Remove(participant.ParticipantId)) return;
            }

            await UpdateUsersTyping();
        }

        public async ValueTask SetUserIsTyping(IServiceMessage<bool> message)
        {
            using var _ = _logger.BeginMethodScope(message.GetScopeData());

            _logger.LogDebug("is typing: {typing}", message.Payload);

            lock (_currentlyTypingLock)
            {
                if (message.Payload)
                {
                    var now = DateTimeOffset.UtcNow;
                    _currentlyTyping[message.Participant.ParticipantId] = now;
                    _logger.LogDebug("Update participant currently typing to {time}", now);
                }
                else
                {
                    if (!_currentlyTyping.Remove(message.Participant.ParticipantId))
                    {
                        _logger.LogDebug("Participant did not exist in currentlyTyping, return");
                        return;
                    }

                    _logger.LogDebug("Removed participant from currently typing.");
                }
            }

            await UpdateUsersTyping();
        }

        public async ValueTask SendMessage(IServiceMessage<SendChatMessageDto> message)
        {
            using var _ = _logger.BeginMethodScope(message.GetScopeData());

            _logger.LogDebug("Message: {@message}", message.Payload);
            var messageDto = message.Payload;

            if (string.IsNullOrWhiteSpace(messageDto.Message))
            {
                _logger.LogDebug("Message is empty, return error");
                await message.ResponseError(ChatError.EmptyMessageNotAllowed);
                return;
            }

            var permissions = await _permissionsService.GetPermissions(message.Participant);
            if (!await permissions.GetPermission(PermissionsList.Chat.CanSendChatMessage))
            {
                _logger.LogDebug("Permissions to send chat message denied");
                await message.ResponseError(ChatError.PermissionToSendMessageDenied);
                return;
            }

            // here would be the point to implement e. g. language filter

            if (messageDto.Mode != null)
                if (messageDto.Mode is SendAnonymously)
                {
                    if (!await permissions.GetPermission(PermissionsList.Chat.CanSendAnonymousMessage))
                    {
                        await message.ResponseError(ChatError.PermissionToSendAnonymousMessageDenied);
                        return;
                    }
                }
                else if (messageDto.Mode is SendPrivately sendPrivately)
                {
                    // ReSharper disable once SimplifyLinqExpressionUseAll
                    if (sendPrivately.ToParticipant == null ||
                        !_connectionMapping.ConnectionsR.ContainsKey(sendPrivately.ToParticipant))
                    {
                        await message.ResponseError(ChatError.InvalidParticipant);
                        return;
                    }

                    if (!await permissions.GetPermission(PermissionsList.Chat.CanSendPrivateChatMessage))
                    {
                        await message.ResponseError(ChatError.PermissionToSendPrivateMessageDenied);
                        return;
                    }
                }
                else
                {
                    _logger.LogDebug("Invalid chat message mode: {@mode}", messageDto.Mode);
                    await message.ResponseError(ChatError.InvalidMode);
                }

            var filter = FilterFactory.CreateFilter(messageDto.Mode, message.Participant.ParticipantId);
            ChatMessage chatMessage;

            _messagesLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                chatMessage = new ChatMessage(_messageIdCounter++, message.Participant.ParticipantId,
                    messageDto.Message, filter, messageDto.Mode, DateTimeOffset.UtcNow);

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
            await chatMessage.Filter.SendAsync(_signalMessenger, _connectionMapping, _conferenceId,
                CoreHubMessages.Response.ChatMessage, dto);
        }

        public ValueTask<IReadOnlyList<ChatMessageDto>> RequestAllMessages(IServiceMessage message)
        {
            List<ChatMessage> messages;

            _messagesLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                messages = _messages.Where(x => x.Filter.ShowMessageTo(message.Participant.ParticipantId))
                    .ToList(); // copy
            }
            finally
            {
                _messagesLock.ReleaseReaderLock();
            }

            return messages.Select(_mapper.Map<ChatMessageDto>).ToList().ToValueTask<IReadOnlyList<ChatMessageDto>>();
        }

        private async ValueTask UpdateUsersTyping()
        {
            using var _ = _logger.BeginMethodScope();

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

        private void OnRefreshUsersTyping(object sender, ElapsedEventArgs e)
        {
            using var _ = _logger.BeginMethodScope();

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