using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core;
using PaderConference.Core.Dto;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Core.Services.BreakoutRoom;
using PaderConference.Core.Services.BreakoutRoom.Dto;
using PaderConference.Core.Services.BreakoutRoom.Requests;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Dto;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Equipment;
using PaderConference.Core.Services.Equipment.Data;
using PaderConference.Core.Services.Equipment.Dto;
using PaderConference.Core.Services.Media;
using PaderConference.Core.Services.Media.Communication;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Permissions.Dto;
using PaderConference.Core.Services.Permissions.Requests;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Requests;
using PaderConference.Core.Signaling;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Services;

namespace PaderConference.Infrastructure.Hubs
{
    [Authorize]
    public class CoreHub : Hub
    {
        private readonly IJoinConferenceUseCase _joinConferenceUseCase;
        private readonly ILeaveConferenceUseCase _leaveConferenceUseCase;
        private readonly IServiceInvoker _invoker;
        private readonly ILogger<CoreHub> _logger;

        public CoreHub(IJoinConferenceUseCase joinConferenceUseCase, ILeaveConferenceUseCase leaveConferenceUseCase,
            IServiceInvokerFactory serviceInvokerFactory, ILogger<CoreHub> logger)
        {
            _joinConferenceUseCase = joinConferenceUseCase;
            _leaveConferenceUseCase = leaveConferenceUseCase;
            _invoker = serviceInvokerFactory.CreateForHub(this);
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            using (_logger.BeginMethodScope(new Dictionary<string, object> {{"connectionId", Context.ConnectionId}}))
            {
                _logger.LogDebug("Client tries to connect");

                SuccessOrError result;
                try
                {
                    result = await HandleJoin();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred when trying to join");
                    result = ConferenceError.UnexpectedError("An unexpected error occurred");
                }

                if (!result.Success)
                {
                    _logger.LogWarning("Client join was not successful: {@error}", result.Error);

                    await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConnectionError, result.Error);
                    Context.Abort();
                }
            }
        }

        private async Task<SuccessOrError> HandleJoin()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var conferenceId = httpContext.Request.Query["conferenceId"].ToString();
                var participantId = httpContext.User.GetUserId();
                var role = httpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;
                var name = httpContext.User.Identity?.Name;

                using var _ = _logger.BeginMethodScope(new Dictionary<string, object>
                {
                    {"conferenceId", conferenceId}, {"participantId", participantId}, {"role", role},
                });

                var login = await _joinConferenceUseCase.Handle(new JoinConferenceRequest(conferenceId, participantId,
                    role, name, Context.ConnectionId, Context.ConnectionAborted,
                    () => Groups.AddToGroupAsync(Context.ConnectionId, conferenceId)));

                if (!login.Success)
                    return login.Error;

                return SuccessOrError.Succeeded;
            }

            _logger.LogError("HttpContext is null");
            return ConferenceError.UnexpectedError("An unexpected error occurred: HttpContext is null");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogDebug(exception, "Connection {connectionId} disconnected", Context.ConnectionId);
            await _leaveConferenceUseCase.Handle(new LeaveConferenceRequest(Context.ConnectionId));
        }

        public Task<SuccessOrError> OpenConference()
        {
            return _invoker.InvokeService<ConferenceControlService>(service => service.OpenConference,
                new MethodOptions {ConferenceCanBeClosed = true});
        }

        public Task<SuccessOrError> CloseConference()
        {
            return _invoker.InvokeService<ConferenceControlService>(service => service.CloseConference);
        }

        public Task<SuccessOrError> KickParticipant(KickParticipantRequest message)
        {
            return _invoker.InvokeService<ConferenceControlService, KickParticipantRequest>(message,
                service => service.KickParticipant);
        }

        public Task<SuccessOrError<ParticipantPermissionInfo>> FetchPermissions(string? participantId)
        {
            return _invoker.InvokeService<PermissionsService, string?, ParticipantPermissionInfo>(participantId,
                service => service.FetchPermissions);
        }

        public Task<SuccessOrError> SetTemporaryPermission(SetTemporaryPermissionRequest dto)
        {
            return _invoker.InvokeService<PermissionsService, SetTemporaryPermissionRequest>(dto,
                service => service.SetTemporaryPermission);
        }

        public Task<SuccessOrError> CreateRooms(IReadOnlyList<CreateRoomMessage> dto)
        {
            return _invoker.InvokeService<RoomsService, IReadOnlyList<CreateRoomMessage>>(dto,
                service => service.CreateRooms);
        }

        public Task<SuccessOrError> RemoveRooms(IReadOnlyList<string> dto)
        {
            return _invoker.InvokeService<RoomsService, IReadOnlyList<string>>(dto, service => service.RemoveRooms);
        }

        public Task<SuccessOrError> OpenBreakoutRooms(OpenBreakoutRoomsRequest request)
        {
            return _invoker.InvokeService<BreakoutRoomService, OpenBreakoutRoomsRequest>(request,
                service => service.OpenBreakoutRooms);
        }

        public Task<SuccessOrError> CloseBreakoutRooms()
        {
            return _invoker.InvokeService<BreakoutRoomService>(service => service.CloseBreakoutRooms);
        }

        public Task<SuccessOrError> ChangeBreakoutRooms(JsonPatchDocument<BreakoutRoomsOptions> dto)
        {
            // convert timestamp value from string to actual timestamp
            var timespanPatchOp = dto.Operations.FirstOrDefault(x => x.path == "/duration");
            if (timespanPatchOp?.value != null)
                timespanPatchOp.value = XmlConvert.ToTimeSpan((string) timespanPatchOp.value);

            return _invoker.InvokeService<BreakoutRoomService, JsonPatchDocument<BreakoutRoomsOptions>>(dto,
                service => service.ChangeBreakoutRooms);
        }

        public Task<SuccessOrError> SwitchRoom(SwitchRoomRequest dto)
        {
            return _invoker.InvokeService<RoomsService, SwitchRoomRequest>(dto, service => service.SwitchRoom);
        }

        public Task<SuccessOrError> SendChatMessage(SendChatMessageRequest dto)
        {
            return _invoker.InvokeService<ChatService, SendChatMessageRequest>(dto, service => service.SendMessage);
        }

        public Task<SuccessOrError<IReadOnlyList<ChatMessageDto>>> RequestChat()
        {
            return _invoker.InvokeService<ChatService, IReadOnlyList<ChatMessageDto>>(
                service => service.FetchMyMessages);
        }

        public Task<SuccessOrError> SetUserIsTyping(bool isTyping)
        {
            return _invoker.InvokeService<ChatService, bool>(isTyping, service => service.SetUserIsTyping);
        }

        public Task<SuccessOrError<JsonElement?>> RequestRouterCapabilities()
        {
            return _invoker.InvokeService<MediaService, JsonElement?>(service => service.GetRouterCapabilities,
                new MethodOptions {ConferenceCanBeClosed = true});
        }

        public Task<SuccessOrError<JsonElement?>> InitializeConnection(JsonElement element)
        {
            return _invoker.InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect<JsonElement>(RedisChannels.Media.Request.InitializeConnection),
                new MethodOptions {ConferenceCanBeClosed = true});
        }

        public Task<SuccessOrError<JsonElement?>> CreateWebRtcTransport(JsonElement element)
        {
            return _invoker.InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect<JsonElement>(RedisChannels.Media.Request.CreateTransport),
                new MethodOptions {ConferenceCanBeClosed = true});
        }

        public Task<SuccessOrError<JsonElement?>> ConnectWebRtcTransport(JsonElement element)
        {
            return _invoker.InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect<JsonElement>(RedisChannels.Media.Request.ConnectTransport));
        }

        public Task<SuccessOrError<JsonElement?>> ProduceWebRtcTransport(JsonElement element)
        {
            return _invoker.InvokeService<MediaService, JsonElement, JsonElement?>(element,
                service => service.Redirect<JsonElement>(RedisChannels.Media.Request.TransportProduce));
        }

        public Task<SuccessOrError<JsonElement?>> ChangeStream(ChangeStreamDto dto)
        {
            return _invoker.InvokeService<MediaService, ChangeStreamDto, JsonElement?>(dto,
                service => service.Redirect<ChangeStreamDto>(RedisChannels.Media.Request.ChangeStream));
        }

        public Task<SuccessOrError<JsonElement?>> ChangeProducerSource(ChangeParticipantProducerSourceDto dto)
        {
            return _invoker.InvokeService<MediaService, ChangeParticipantProducerSourceDto, JsonElement?>(dto,
                service => service.RedirectChangeProducerSource(RedisChannels.Media.Request.ChangeProducerSource));
        }

        public Task<SuccessOrError<string>> GetEquipmentToken()
        {
            return _invoker.InvokeService<EquipmentService, string>(service => service.GetEquipmentToken,
                new MethodOptions {ConferenceCanBeClosed = true});
        }

        public Task<SuccessOrError> RegisterEquipment(RegisterEquipmentRequestDto dto)
        {
            return _invoker.InvokeService<EquipmentService, RegisterEquipmentRequestDto>(dto,
                service => service.RegisterEquipment);
        }

        public Task<SuccessOrError> SendEquipmentCommand(EquipmentCommand dto)
        {
            return _invoker.InvokeService<EquipmentService, EquipmentCommand>(dto,
                service => service.SendEquipmentCommand);
        }

        public Task<SuccessOrError> EquipmentErrorOccurred(Error dto)
        {
            return _invoker.InvokeService<EquipmentService, Error>(dto, service => service.EquipmentErrorOccurred);
        }

        public Task<SuccessOrError> EquipmentUpdateStatus(Dictionary<string, UseMediaStateInfo> dto)
        {
            return _invoker.InvokeService<EquipmentService, Dictionary<string, UseMediaStateInfo>>(dto,
                service => service.EquipmentUpdateStatus);
        }
    }
}