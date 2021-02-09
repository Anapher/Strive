using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Permissions;
using PaderConference.Extensions;
using PaderConference.Hubs.Dtos;
using PaderConference.Hubs.Services;
using PaderConference.Hubs.Services.Middlewares;
using PaderConference.Infrastructure.Extensions;

namespace PaderConference.Hubs
{
    [Authorize]
    public class CoreHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IComponentContext _context;
        private readonly ICoreHubConnections _connections;
        private readonly ILogger<CoreHub> _logger;

        public CoreHub(IMediator mediator, IComponentContext context, ICoreHubConnections connections,
            ILogger<CoreHub> logger)
        {
            _mediator = mediator;
            _context = context;
            _connections = connections;
            _logger = logger;
        }

        private IServiceInvoker GetInvoker()
        {
            var (conferenceId, participantId) = GetContextInfo();
            return new ServiceInvoker(_mediator,
                new ServiceInvokerContext(this, _context, conferenceId, participantId));
        }

        public override async Task OnConnectedAsync()
        {
            using (_logger.BeginMethodScope(new Dictionary<string, object> {{"connectionId", Context.ConnectionId}}))
            {
                _logger.LogDebug("Client tries to connect");

                try
                {
                    await HandleJoin();
                }
                catch (Exception e)
                {
                    var error = e.ToError();
                    _logger.LogWarning("Client join was not successful: {@error}", error);

                    await Clients.Caller.SendAsync(CoreHubMessages.Response.OnConnectionError, error);
                    Context.Abort();
                }
            }
        }

        private async Task HandleJoin()
        {
            var (conferenceId, participantId) = GetContextInfo();
            var connectionId = Context.ConnectionId;

            await _mediator.Send(new JoinConferenceRequest(conferenceId, participantId, connectionId),
                Context.ConnectionAborted);

            _connections.SetParticipant(participantId, new ParticipantConnection(conferenceId, Context.ConnectionId));
        }

        private (string conferenceId, string participantId) GetContextInfo()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
                throw ConferenceError.UnexpectedError("An unexpected error occurred: HttpContext is null")
                    .ToException();

            var conferenceId = httpContext.Request.Query["conferenceId"].ToString();
            var participantId = httpContext.User.GetUserId();

            return (conferenceId, participantId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogDebug(exception, "Connection {connectionId} disconnected", Context.ConnectionId);

            var (conferenceId, participantId) = GetContextInfo();
            var connectionId = Context.ConnectionId;

            await _mediator.Publish(new ParticipantLeftNotification(participantId, conferenceId, connectionId));
            _connections.RemoveParticipant(participantId);
        }

        public Task<SuccessOrError<Unit>> OpenConference()
        {
            var (conferenceId, _) = GetContextInfo();
            return GetInvoker().Create(new OpenConferenceRequest(conferenceId))
                .RequirePermissions(DefinedPermissions.Conference.CanOpenAndClose).Send();
        }

        public Task<SuccessOrError<Unit>> CloseConference()
        {
            var (conferenceId, _) = GetContextInfo();
            return GetInvoker().Create(new CloseConferenceRequest(conferenceId))
                .RequirePermissions(DefinedPermissions.Conference.CanOpenAndClose).Send();
        }

        public Task<SuccessOrError<Unit>> KickParticipant(KickParticipantRequestDto message)
        {
            var (conferenceId, _) = GetContextInfo();
            return GetInvoker().Create(new KickParticipantRequest(message.ParticipantId, conferenceId))
                .ValidateObject(message).RequirePermissions(DefinedPermissions.Conference.CanKickParticipant).Send();
        }

        //public Task<SuccessOrError<ParticipantPermissionDto>> FetchPermissions(string? participantId)
        //{
        //    return _invoker.InvokeService<PermissionsService, string?, ParticipantPermissionDto>(participantId,
        //        service => service.FetchPermissions);
        //}

        //public Task<SuccessOrError> SetTemporaryPermission(SetTemporaryPermissionRequest dto)
        //{
        //    return _invoker.InvokeService<PermissionsService, SetTemporaryPermissionRequest>(dto,
        //        service => service.SetTemporaryPermission);
        //}

        //public Task<SuccessOrError> CreateRooms(IReadOnlyList<CreateRoomMessage> dto)
        //{
        //    return _invoker.InvokeService<RoomsService, IReadOnlyList<CreateRoomMessage>>(dto,
        //        service => service.CreateRooms);
        //}

        //public Task<SuccessOrError> RemoveRooms(IReadOnlyList<string> dto)
        //{
        //    return _invoker.InvokeService<RoomsService, IReadOnlyList<string>>(dto, service => service.RemoveRooms);
        //}

        //public Task<SuccessOrError> OpenBreakoutRooms(OpenBreakoutRoomsRequest request)
        //{
        //    return _invoker.InvokeService<BreakoutRoomService, OpenBreakoutRoomsRequest>(request,
        //        service => service.OpenBreakoutRooms);
        //}

        //public Task<SuccessOrError> CloseBreakoutRooms()
        //{
        //    return _invoker.InvokeService<BreakoutRoomService>(service => service.CloseBreakoutRooms);
        //}

        //public Task<SuccessOrError> ChangeBreakoutRooms(JsonPatchDocument<BreakoutRoomsOptions> dto)
        //{
        //    // convert timestamp value from string to actual timestamp
        //    var timespanPatchOp = dto.Operations.FirstOrDefault(x => x.path == "/duration");
        //    if (timespanPatchOp?.value != null)
        //        timespanPatchOp.value = XmlConvert.ToTimeSpan((string) timespanPatchOp.value);

        //    return _invoker.InvokeService<BreakoutRoomService, JsonPatchDocument<BreakoutRoomsOptions>>(dto,
        //        service => service.ChangeBreakoutRooms);
        //}

        //public Task<SuccessOrError> SwitchRoom(SwitchRoomRequest dto)
        //{
        //    return _invoker.InvokeService<RoomsService, SwitchRoomRequest>(dto, service => service.SwitchRoom);
        //}

        //public Task<SuccessOrError> SendChatMessage(SendChatMessageRequest dto)
        //{
        //    return _invoker.InvokeService<ChatService, SendChatMessageRequest>(dto, service => service.SendMessage);
        //}

        //public Task<SuccessOrError<IReadOnlyList<ChatMessageDto>>> RequestChat()
        //{
        //    return _invoker.InvokeService<ChatService, IReadOnlyList<ChatMessageDto>>(
        //        service => service.FetchMyMessages);
        //}

        //public Task<SuccessOrError> SetUserIsTyping(bool isTyping)
        //{
        //    return _invoker.InvokeService<ChatService, bool>(isTyping, service => service.SetUserIsTyping);
        //}

        //public Task<SuccessOrError<string>> GetEquipmentToken()
        //{
        //    return _invoker.InvokeService<EquipmentService, string>(service => service.GetEquipmentToken,
        //        new MethodOptions {ConferenceCanBeClosed = true});
        //}

        //public Task<SuccessOrError> RegisterEquipment(RegisterEquipmentRequestDto dto)
        //{
        //    return _invoker.InvokeService<EquipmentService, RegisterEquipmentRequestDto>(dto,
        //        service => service.RegisterEquipment);
        //}

        //public Task<SuccessOrError> SendEquipmentCommand(EquipmentCommand dto)
        //{
        //    return _invoker.InvokeService<EquipmentService, EquipmentCommand>(dto,
        //        service => service.SendEquipmentCommand);
        //}

        //public Task<SuccessOrError> EquipmentErrorOccurred(Error dto)
        //{
        //    return _invoker.InvokeService<EquipmentService, Error>(dto, service => service.EquipmentErrorOccurred);
        //}

        //public Task<SuccessOrError> EquipmentUpdateStatus(Dictionary<string, UseMediaStateInfo> dto)
        //{
        //    return _invoker.InvokeService<EquipmentService, Dictionary<string, UseMediaStateInfo>>(dto,
        //        service => service.EquipmentUpdateStatus);
        //}
    }
}
