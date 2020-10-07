using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.Infrastructure.Services;
using PaderConference.Infrastructure.Services.Chat;
using PaderConference.Infrastructure.Services.Media;
using PaderConference.Infrastructure.Services.Media.Data;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure.Hubs
{
    [Authorize]
    public class CoreHub : HubBase
    {
        private readonly IConferenceManager _conferenceManager;
        private readonly IEnumerable<IConferenceServiceManager> _conferenceServices;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<CoreHub> _logger;

        public CoreHub(IConferenceManager conferenceManager, IConnectionMapping connectionMapping,
            IEnumerable<IConferenceServiceManager> conferenceServices, ILogger<CoreHub> logger) : base(
            connectionMapping, logger)
        {
            _conferenceManager = conferenceManager;
            _logger = logger;
            _connectionMapping = connectionMapping;
            _conferenceServices = conferenceServices;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var conferenceId = httpContext.Request.Query["conferenceId"].ToString();
                var userId = httpContext.User.GetUserId();
                var role = httpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                _logger.LogDebug(
                    "Client tries to connect (user: {userId}, connectionId: {connectionId}) to conference {conferenceId}",
                    userId,
                    Context.ConnectionId, conferenceId);

                Participant participant;
                try
                {
                    participant =
                        await _conferenceManager.Participate(conferenceId, userId, role,
                            httpContext.User.Identity.Name);
                }
                catch (InvalidOperationException)
                {
                    _logger.LogDebug("Conference {conferenceId} was not found. Abort connection", conferenceId);

                    await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConferenceDoesNotExist);
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

                foreach (var service in _conferenceServices)
                    await service.GetService(participant.Conference, _conferenceServices)
                        .OnClientConnected(participant);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (!_connectionMapping.Connections.TryGetValue(Context.ConnectionId, out var participant))
                return;

            _connectionMapping.Remove(Context.ConnectionId);
            await _conferenceManager.RemoveParticipant(participant);

            foreach (var service in _conferenceServices)
                await service.GetService(participant.Conference, _conferenceServices).OnClientDisconnected(participant);
        }

        private T GetConferenceService<T>(Conference conference) where T : IConferenceService
        {
            return _conferenceServices.OfType<IConferenceServiceManager<T>>().First()
                .GetService(conference, _conferenceServices);
        }

        public async Task SendChatMessage(SendChatMessageDto dto)
        {
            if (GetMessage(dto, out var message))
                await GetConferenceService<ChatService>(message.Participant.Conference)
                    .SendMessage(message);
        }

        public async Task RequestChat()
        {
            if (GetMessage(out var message))
                await GetConferenceService<ChatService>(message.Participant.Conference).RequestAllMessages(message);
        }

        public async Task RtcSendIceCandidate(RTCIceCandidate iceCandidate)
        {
            if (GetMessage(iceCandidate, out var message))
                await GetConferenceService<MediaService>(message.Participant.Conference).OnIceCandidate(message);
        }

        public async Task RtcSetDescription(RTCSessionDescription dto)
        {
            if (GetMessage(dto, out var message))
                await GetConferenceService<MediaService>(message.Participant.Conference).SetDescription(message);
        }

        public async Task RequestVideo()
        {
            if (GetMessage(out var message))
                await GetConferenceService<MediaService>(message.Participant.Conference).RequestVideo(message);
        }
    }
}