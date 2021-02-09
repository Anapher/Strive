using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core;
using PaderConference.Core.Dto;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Permissions;
using PaderConference.Infrastructure.Extensions;

namespace PaderConference.Hubs
{
    [Authorize]
    public class CoreHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IParticipantPermissions _participantPermissions;
        private readonly ILogger<CoreHub> _logger;

        public CoreHub(IMediator mediator, IParticipantPermissions participantPermissions, ILogger<CoreHub> logger)
        {
            _mediator = mediator;
            _participantPermissions = participantPermissions;
            _logger = logger;
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
                    var error = ExceptionToError(e);
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


        private static Error ExceptionToError(Exception e)
        {
            if (e is IdErrorException idError)
                return idError.Error;

            return ConferenceError.UnexpectedError("An unexpected error occurred");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogDebug(exception, "Connection {connectionId} disconnected", Context.ConnectionId);

            var (conferenceId, participantId) = GetContextInfo();
            var connectionId = Context.ConnectionId;

            await _mediator.Publish(new ParticipantLeftNotification(participantId, conferenceId, connectionId));
        }

        private async Task<SuccessOrError<TResponse>> WrapMediatorSend<TResponse>(IRequest<TResponse> request)
        {
            try
            {
                var result = await _mediator.Send(request, Context.ConnectionAborted);
                return SuccessOrError<TResponse>.Succeeded(result);
            }
            catch (Exception e)
            {
                return ExceptionToError(e);
            }
        }

        private async Task<SuccessOrError<TResponse>> ExecuteIfPermissions<TResponse>(
            Func<Task<SuccessOrError<TResponse>>> action, params PermissionDescriptor<bool>[] requiredPermissions)
        {
            var (conferenceId, participantId) = GetContextInfo();
            var permissions = await _participantPermissions.FetchForParticipant(conferenceId, participantId);

            foreach (var permission in requiredPermissions)
            {
                var permissionValue = await permissions.GetPermissionValue(permission);
                if (!permissionValue) return CommonError.PermissionDenied(permission);
            }

            return await action();
        }

        public Task<SuccessOrError<Unit>> OpenConference()
        {
            var (conferenceId, _) = GetContextInfo();
            return ExecuteIfPermissions(() => WrapMediatorSend(new OpenConferenceRequest(conferenceId)),
                DefinedPermissions.Conference.CanOpenAndClose);
        }

        public Task<SuccessOrError<Unit>> CloseConference()
        {
            var (conferenceId, _) = GetContextInfo();
            return ExecuteIfPermissions(() => WrapMediatorSend(new CloseConferenceRequest(conferenceId)),
                DefinedPermissions.Conference.CanOpenAndClose);
        }

        //public Task<SuccessOrError> KickParticipant(KickParticipantRequest message)
        //{
        //    return _invoker.InvokeService<ConferenceControlService, KickParticipantRequest>(message,
        //        service => service.KickParticipant);
        //}

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
