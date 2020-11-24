using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using PaderConference.Core.Services.BreakoutRoom;
using PaderConference.Core.Services.BreakoutRoom.Dto;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Dto;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.Equipment;
using PaderConference.Core.Services.Equipment.Data;
using PaderConference.Core.Services.Equipment.Dto;
using PaderConference.Core.Services.Media;
using PaderConference.Core.Services.Media.Communication;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Messages;
using PaderConference.Core.Signaling;
using PaderConference.Infrastructure.Conferencing;
using PaderConference.Infrastructure.Extensions;

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
                    var participantId = httpContext.User.GetUserId();
                    var role = httpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                    using (_logger.BeginScope(new Dictionary<string, object>
                    {
                        {"conferenceId", conferenceId}, {"participantId", participantId}, {"role", role},
                    }))
                    {
                        _logger.LogDebug("Client tries to connect");

                        // initialize all services before submitting events
                        var services = ConferenceServices.Select(x => x.GetService(conferenceId)).ToList();
                        foreach (var valueTask in services) await valueTask;

                        switch (role)
                        {
                            case PrincipalRoles.Equipment:
                            {
                                if (!ConferenceManager.TryGetParticipant(conferenceId, participantId,
                                    out var participant))
                                {
                                    await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConnectionError,
                                        ConferenceError.UnexpectedError(
                                            "Participant is not connected to this conference."));
                                    Context.Abort();
                                    return;
                                }

                                if (!_connectionMapping.Add(Context.ConnectionId, participant, true))
                                {
                                    _logger.LogError(
                                        "Participant {participantId} could not be added to connection mapping.",
                                        participant.ParticipantId);
                                    await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConnectionError,
                                        ConferenceError.UnexpectedError(
                                            "Participant could not be added to connection mapping."));
                                    Context.Abort();
                                    return;
                                }

                                var equipmentService = await ConferenceServices
                                    .OfType<IConferenceServiceManager<EquipmentService>>().First()
                                    .GetService(conferenceId);

                                await equipmentService.OnEquipmentConnected(participant, Context.ConnectionId);
                                break;
                            }
                            case PrincipalRoles.Moderator:
                            case PrincipalRoles.User:
                            case PrincipalRoles.Guest:
                            {
                                Participant participant;
                                try
                                {
                                    participant = await ConferenceManager.Participate(conferenceId, participantId, role,
                                        httpContext.User.Identity.Name);
                                }
                                catch (ConferenceNotFoundException)
                                {
                                    _logger.LogDebug("Abort connection");
                                    await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConnectionError,
                                        ConferenceError.NotFound);
                                    Context.Abort();
                                    return;
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, "Unexpected exception occurred.");
                                    await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConnectionError,
                                        ConferenceError.UnexpectedError(e.Message));
                                    Context.Abort();
                                    return;
                                }

                                if (!_connectionMapping.Add(Context.ConnectionId, participant))
                                {
                                    _logger.LogError(
                                        "Participant {participantId} could not be added to connection mapping.",
                                        participant.ParticipantId);
                                    await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConnectionError,
                                        ConferenceError.UnexpectedError(
                                            "Participant could not be added to connection mapping."));
                                    Context.Abort();
                                    return;
                                }

                                foreach (var service in services)
                                    await service.Result.OnClientConnected(participant);

                                await Groups.AddToGroupAsync(Context.ConnectionId, conferenceId);

                                foreach (var service in services)
                                    await service.Result.InitializeParticipant(participant);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (!_connectionMapping.Connections.TryGetValue(Context.ConnectionId, out var participant))
                return;

            var conferenceId = ConferenceManager.GetConferenceOfParticipant(participant);

            var role = Context.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;
            if (role == PrincipalRoles.Equipment)
            {
                var equipmentService = await ConferenceServices.OfType<IConferenceServiceManager<EquipmentService>>()
                    .First().GetService(conferenceId);

                await equipmentService.OnEquipmentDisconnected(participant, Context.ConnectionId);
                _connectionMapping.Remove(Context.ConnectionId);
                return;
            }

            // important for participants list, else the disconnected participant will still be in the list
            await ConferenceManager.RemoveParticipant(participant);

            // Todo: close conference if it was the last participant and some time passed
            foreach (var service in ConferenceServices)
                await (await service.GetService(conferenceId)).OnClientDisconnected(participant);

            _connectionMapping.Remove(Context.ConnectionId);
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

        public Task OpenBreakoutRooms(OpenBreakoutRoomsDto dto)
        {
            return InvokeService<BreakoutRoomService, OpenBreakoutRoomsDto>(dto, service => service.OpenBreakoutRooms);
        }

        public Task CloseBreakoutRooms()
        {
            return InvokeService<BreakoutRoomService>(service => service.CloseBreakoutRooms);
        }

        public Task ChangeBreakoutRooms(JsonPatchDocument<BreakoutRoomsOptions> dto)
        {
            return InvokeService<BreakoutRoomService, JsonPatchDocument<BreakoutRoomsOptions>>(dto,
                service => service.ChangeBreakoutRooms);
        }

        public Task SwitchRoom(SwitchRoomMessage dto)
        {
            return InvokeService<RoomsService, SwitchRoomMessage>(dto, service => service.SwitchRoom);
        }

        public Task SendChatMessage(SendChatMessageDto dto)
        {
            return InvokeService<ChatService, SendChatMessageDto>(dto, service => service.SendMessage);
        }

        public Task<IReadOnlyList<ChatMessageDto>> RequestChat()
        {
            return InvokeService<ChatService, IReadOnlyList<ChatMessageDto>>(service => service.RequestAllMessages);
        }

        public Task SetUserIsTyping(bool isTyping)
        {
            return InvokeService<ChatService, bool>(isTyping, service => service.SetUserIsTyping);
        }

        public Task<JsonElement?> RequestRouterCapabilities()
        {
            return InvokeService<MediaService, JsonElement?>(service => service.GetRouterCapabilities);
        }

        public Task InitializeConnection(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect<JsonElement>(RedisChannels.Media.Request.InitializeConnection));
        }

        public Task<JsonElement?> CreateWebRtcTransport(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect<JsonElement>(RedisChannels.Media.Request.CreateTransport));
        }

        public Task<JsonElement?> ConnectWebRtcTransport(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect<JsonElement>(RedisChannels.Media.Request.ConnectTransport));
        }

        public Task<JsonElement?> ProduceWebRtcTransport(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect<JsonElement>(RedisChannels.Media.Request.TransportProduce));
        }

        public Task ChangeStream(ChangeStreamDto dto)
        {
            return InvokeService<MediaService, ChangeStreamDto, JsonElement?>(dto,
                service => service.Redirect<ChangeStreamDto>(RedisChannels.Media.Request.ChangeStream));
        }

        public Task<string> GetEquipmentToken()
        {
            return InvokeService<EquipmentService, string>(service => service.GetEquipmentToken);
        }

        public Task RegisterEquipment(RegisterEquipmentRequestDto dto)
        {
            return InvokeService<EquipmentService, RegisterEquipmentRequestDto>(dto,
                service => service.RegisterEquipment);
        }

        public Task SendEquipmentCommand(EquipmentCommand dto)
        {
            return InvokeService<EquipmentService, EquipmentCommand>(dto, service => service.SendEquipmentCommand);
        }

        public Task EquipmentErrorOccurred(Error dto)
        {
            return InvokeService<EquipmentService, Error>(dto, service => service.EquipmentErrorOccurred);
        }

        public Task EquipmentUpdateStatus(Dictionary<string, UseMediaStateInfo> dto)
        {
            return InvokeService<EquipmentService, Dictionary<string, UseMediaStateInfo>>(dto,
                service => service.EquipmentUpdateStatus);
        }
    }
}