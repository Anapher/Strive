using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Conferencing;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.Infrastructure.Services;
using PaderConference.Infrastructure.Services.Chat;
using PaderConference.Infrastructure.Services.ConferenceControl;
using PaderConference.Infrastructure.Services.Media;
using PaderConference.Infrastructure.Services.Rooms;
using PaderConference.Infrastructure.Services.Rooms.Messages;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure.Hubs
{
    [Authorize]
    public class CoreHub : ServiceHubBase
    {
        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<CoreHub> _logger;

        public CoreHub(IConferenceManager conferenceManager, IConnectionMapping connectionMapping,
            IEnumerable<IConferenceServiceManager> conferenceServices, ILogger<CoreHub> logger) : base(
            connectionMapping, conferenceManager, conferenceServices, logger)
        {
            _logger = logger;
            _connectionMapping = connectionMapping;
        }

        public override async Task OnConnectedAsync()
        {
            using (_logger.BeginScope($"{nameof(OnConnectedAsync)}()"))
            using (_logger.BeginScope(new Dictionary<string, object> {{"connectionId", Context.ConnectionId}}))
            {
                var httpContext = Context.GetHttpContext();
                if (httpContext != null)
                {
                    var conferenceId = httpContext.Request.Query["conferenceId"].ToString();
                    var userId = httpContext.User.GetUserId();
                    var role = httpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                    using (_logger.BeginScope(new Dictionary<string, object>
                    {
                        {"conferenceId", conferenceId}, {"userId", userId}, {"role", role}
                    }))
                    {
                        _logger.LogDebug("Client tries to connect");

                        Participant participant;
                        try
                        {
                            participant = await ConferenceManager.Participate(conferenceId, userId, role,
                                httpContext.User.Identity.Name);
                        }
                        catch (ConferenceNotFoundException)
                        {
                            _logger.LogDebug("Abort connection");
                            await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConferenceJoinError,
                                ConferenceError.NotFound);
                            Context.Abort();
                            return;
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Unexpected exception occurred.");
                            await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConferenceJoinError,
                                ConferenceError.UnexpectedError(e.Message));
                            Context.Abort();
                            return;
                        }

                        if (!_connectionMapping.Add(Context.ConnectionId, participant))
                        {
                            _logger.LogError("Participant {participantId} could not be added to connection mapping.",
                                participant.ParticipantId);
                            await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConferenceJoinError,
                                ConferenceError.UnexpectedError(
                                    "Participant could not be added to connection mapping."));
                            Context.Abort();
                            return;
                        }

                        // initialize all services before submitting events
                        var services = ConferenceServices.Select(x => x.GetService(conferenceId, ConferenceServices))
                            .ToList();
                        foreach (var valueTask in services) await valueTask;

                        foreach (var service in services)
                            await service.Result.OnClientConnected(participant);

                        await Groups.AddToGroupAsync(Context.ConnectionId, conferenceId);

                        foreach (var service in services)
                            await service.Result.InitializeParticipant(participant);
                    }
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Todo: close conference if it was the last participant and some time passed
            if (!_connectionMapping.Connections.TryGetValue(Context.ConnectionId, out var participant))
                return;

            var conferenceId = ConferenceManager.GetConferenceOfParticipant(participant);

            foreach (var service in ConferenceServices)
                await (await service.GetService(conferenceId, ConferenceServices)).OnClientDisconnected(participant);

            _connectionMapping.Remove(Context.ConnectionId);
            await ConferenceManager.RemoveParticipant(participant);
        }

        public Task OpenConference()
        {
            return InvokeService<ConferenceControlService>(service => service.OpenConference,
                new MethodOptions {ConferenceCanBeClosed = true});
        }

        public Task CloseConference()
        {
            return InvokeService<ConferenceControlService>(service => service.CloseConference);
        }

        public Task CreateRooms(IReadOnlyList<CreateRoomMessage> dto)
        {
            return InvokeService<RoomsService, IReadOnlyList<CreateRoomMessage>>(dto, service => service.CreateRooms);
        }

        public Task RemoveRooms(IReadOnlyList<string> dto)
        {
            return InvokeService<RoomsService, IReadOnlyList<string>>(dto, service => service.RemoveRooms);
        }

        public Task SwitchRoom(SwitchRoomMessage dto)
        {
            return InvokeService<RoomsService, SwitchRoomMessage>(dto, service => service.SwitchRoom);
        }

        public Task SendChatMessage(SendChatMessageDto dto)
        {
            return InvokeService<ChatService, SendChatMessageDto>(dto, service => service.SendMessage);
        }

        public Task RequestChat()
        {
            return InvokeService<ChatService>(service => service.RequestAllMessages);
        }

        public Task<JsonElement?> RequestRouterCapabilities()
        {
            return InvokeService<MediaService, JsonElement?>(service => service.GetRouterCapabilities);
        }

        public Task<Error?> InitializeConnection(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, Error?>(element,
                service => service.InitializeConnection);
        }

        public Task<JsonElement?> CreateWebRtcTransport(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect(RedisChannels.Media.Request.CreateTransport));
        }

        public Task<JsonElement?> ConnectWebRtcTransport(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect(RedisChannels.Media.Request.ConnectTransport));
        }

        public Task<JsonElement?> ProduceWebRtcTransport(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect(RedisChannels.Media.Request.TransportProduce));
        }
    }
}