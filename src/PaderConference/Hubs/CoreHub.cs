using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Extensions;
using PaderConference.Hubs.Chat;
using PaderConference.Infrastructure.Sockets;
using PaderConference.Models.Signal;

namespace PaderConference.Hubs
{
    [Authorize]
    public class CoreHub : Hub
    {
        private readonly IChatManager _chatManager;
        private readonly IConferenceManager _conferenceManager;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<CoreHub> _logger;

        public CoreHub(IConferenceManager conferenceManager, IConnectionMapping connectionMapping,
            IChatManager chatManager, ILogger<CoreHub> logger)
        {
            _conferenceManager = conferenceManager;
            _logger = logger;
            _connectionMapping = connectionMapping;
            _chatManager = chatManager;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var conferenceId = httpContext.Request.Query["conferenceId"].ToString();
                var userId = httpContext.User.GetUserId();

                _logger.LogDebug(
                    "Client tries to connect (user: {userId}, connectionId: {connectionId}) to conference {conferenceId}",
                    userId,
                    Context.ConnectionId, conferenceId);

                Participant participant;
                try
                {
                    participant = await _conferenceManager.Participate(conferenceId, httpContext.User.Identity.Name);
                }
                catch (InvalidOperationException)
                {
                    _logger.LogDebug("Conference {conferenceId} was not found. Abort connection", conferenceId);
                    Context.Abort();
                    return;
                }

                if (!_connectionMapping.Add(Context.ConnectionId, participant))
                {
                    _logger.LogWarning("Participant {participantId} could not be added to connection mapping.",
                        participant.ParticipantId);
                    Context.Abort();
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, conferenceId);

                //await Clients.Client(Context.ConnectionId).SendAsync("Welcome", "");
                //await Clients.Group(conferenceId).SendAsync("OnUserJoined", "");
            }
        }

        public async Task SendChatMessage(SendChatMessageDto message)
        {
            if (!_connectionMapping.Connections.TryGetValue(Context.ConnectionId, out var participant))
            {
                _logger.LogWarning("Connection {connectionId} is not mapped to a participant.", Context.ConnectionId);
                return;
            }

            var chat = _chatManager.GetChat(participant.Conference);
            await chat.SendMessage(message, Context, Clients);
        }

        public async Task RequestChat()
        {
            if (!_connectionMapping.Connections.TryGetValue(Context.ConnectionId, out var participant))
            {
                _logger.LogWarning("Connection {connectionId} is not mapped to a participant.", Context.ConnectionId);
                return;
            }

            var chat = _chatManager.GetChat(participant.Conference);
            await chat.RequestAllMessages(Context, Clients);
        }
    }
}