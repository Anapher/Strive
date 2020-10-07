using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Extensions;
using PaderConference.Hubs.Chat;
using PaderConference.Hubs.Media;
using PaderConference.Infrastructure.Sockets;
using PaderConference.Models.Signal;

namespace PaderConference.Hubs
{
    [Authorize]
    public class CoreHub : Hub
    {
        private readonly IConferenceServiceManager<ChatService> _chatManager;
        private readonly IConferenceManager _conferenceManager;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<CoreHub> _logger;
        private readonly IMapper _mapper;
        private readonly IConferenceServiceManager<MediaService> _mediaManager;

        public CoreHub(IConferenceManager conferenceManager, IConnectionMapping connectionMapping,
            IConferenceServiceManager<ChatService> chatManager, IConferenceServiceManager<MediaService> mediaManager,
            IMapper mapper, ILogger<CoreHub> logger)
        {
            _conferenceManager = conferenceManager;
            _logger = logger;
            _connectionMapping = connectionMapping;
            _chatManager = chatManager;
            _mediaManager = mediaManager;
            _mapper = mapper;
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
                await UpdateParticipants(participant.Conference);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (!_connectionMapping.Connections.TryGetValue(Context.ConnectionId, out var participant))
                return;

            _connectionMapping.Remove(Context.ConnectionId);
            await _conferenceManager.RemoveParticipant(participant);

            await UpdateParticipants(participant.Conference);

            await _mediaManager.GetService(participant.Conference).OnClientDisconnected(participant);
            await _chatManager.GetService(participant.Conference).OnClientDisconnected(participant);
        }

        public async Task SendChatMessage(SendChatMessageDto dto)
        {
            if (GetMessage(dto, out var message))
                await _chatManager.GetService(message.Participant.Conference)
                    .SendMessage(message);
        }

        public async Task RequestChat()
        {
            if (GetMessage(out var message))
                await _chatManager.GetService(message.Participant.Conference).RequestAllMessages(message);
        }

        public async Task RtcSendIceCandidate(RTCIceCandidate iceCandidate)
        {
            if (GetMessage(iceCandidate, out var message))
                await _mediaManager.GetService(message.Participant.Conference).OnIceCandidate(message);
        }

        public async Task RtcSetDescription(RTCSessionDescription dto)
        {
            if (GetMessage(dto, out var message))
                await _mediaManager.GetService(message.Participant.Conference).SetDescription(message);
        }

        public async Task RequestVideo()
        {
            if (GetMessage(out var message))
                await _mediaManager.GetService(message.Participant.Conference).RequestVideo(message);
        }

        private async Task UpdateParticipants(Conference conference)
        {
            var participants = conference.Participants.Values.Select(_mapper.Map<ParticipantDto>).ToList();
            await Clients.Group(conference.ConferenceId)
                .SendAsync(CoreHubMessages.Response.OnParticipantsUpdated, participants);
        }

        private bool GetMessage([NotNullWhen(true)] out IServiceMessage? serviceMessage)
        {
            if (AssertParticipant(out var participant))
            {
                serviceMessage = new ServiceMessage(participant, Context, Clients);
                return true;
            }

            serviceMessage = null;
            return false;
        }

        private bool GetMessage<T>(T payload, [NotNullWhen(true)] out IServiceMessage<T>? serviceMessage)
        {
            if (AssertParticipant(out var participant))
            {
                serviceMessage = new ServiceMessage<T>(payload, participant, Context, Clients);
                return true;
            }

            serviceMessage = null;
            return false;
        }

        private bool AssertParticipant([NotNullWhen(true)] out Participant? participant)
        {
            if (!_connectionMapping.Connections.TryGetValue(Context.ConnectionId, out participant))
            {
                _logger.LogWarning("Connection {connectionId} is not mapped to a participant.", Context.ConnectionId);
                return false;
            }

            return true;
        }
    }
}