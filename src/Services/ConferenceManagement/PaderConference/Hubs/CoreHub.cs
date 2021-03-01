using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core;
using PaderConference.Core.Errors;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Permissions.Requests;
using PaderConference.Core.Services.Permissions.Responses;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Requests;
using PaderConference.Core.Utilities;
using PaderConference.Extensions;
using PaderConference.Hubs.Dtos;
using PaderConference.Hubs.Responses;
using PaderConference.Hubs.Services;
using PaderConference.Hubs.Services.Middlewares;
using PaderConference.Hubs.Validators.Extensions;
using PaderConference.Infrastructure.Extensions;

namespace PaderConference.Hubs
{
    [Authorize]
    public class CoreHub : ScopedHub
    {
        private readonly ICoreHubConnections _connections;
        private readonly ILogger<CoreHub> _logger;
        private readonly IMediator _mediator;

        public CoreHub(ILifetimeScope scope) : base(scope)
        {
            _mediator = HubScope.Resolve<IMediator>();
            _connections = HubScope.Resolve<ICoreHubConnections>();
            _logger = HubScope.Resolve<ILogger<CoreHub>>();
        }

        private IServiceInvoker GetInvoker()
        {
            var participant = GetContextParticipant();
            return new ServiceInvoker(_mediator, new ServiceInvokerContext(this, HubScope, participant));
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
            var participant = GetContextParticipant();
            var metadata = GetMetadata();
            var connectionId = Context.ConnectionId;

            await _mediator.Send(new JoinConferenceRequest(participant, connectionId, metadata),
                Context.ConnectionAborted);

            _connections.SetParticipant(participant.Id,
                new ParticipantConnection(participant.ConferenceId, Context.ConnectionId));
        }

        private Participant GetContextParticipant()
        {
            var httpContext = GetHttpContext();

            var conferenceId = httpContext.Request.Query["conferenceId"].ToString();
            var participantId = httpContext.User.GetUserId();

            return new Participant(conferenceId, participantId);
        }

        private ParticipantMetadata GetMetadata()
        {
            var httpContext = GetHttpContext();
            var name = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;

            return new ParticipantMetadata(name);
        }

        private HttpContext GetHttpContext()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
                throw ConferenceError.UnexpectedError("An unexpected error occurred: HttpContext is null")
                    .ToException();

            return httpContext;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogDebug(exception, "Connection {connectionId} disconnected", Context.ConnectionId);

            var participant = GetContextParticipant();
            var connectionId = Context.ConnectionId;

            await _mediator.Publish(new ParticipantLeftNotification(participant, connectionId));
            _connections.RemoveParticipant(participant.Id);
        }

        public Task<SuccessOrError<Unit>> OpenConference()
        {
            var (conferenceId, _) = GetContextParticipant();
            return GetInvoker().Create(new OpenConferenceRequest(conferenceId))
                .RequirePermissions(DefinedPermissions.Conference.CanOpenAndClose).Send();
        }

        public Task<SuccessOrError<Unit>> CloseConference()
        {
            var (conferenceId, _) = GetContextParticipant();
            return GetInvoker().Create(new CloseConferenceRequest(conferenceId))
                .RequirePermissions(DefinedPermissions.Conference.CanOpenAndClose).ConferenceMustBeOpen().Send();
        }

        public Task<SuccessOrError<Unit>> KickParticipant(KickParticipantRequestDto message)
        {
            var (conferenceId, _) = GetContextParticipant();
            return GetInvoker().Create(new KickParticipantRequest(new Participant(conferenceId, message.ParticipantId)))
                .ValidateObject(message).RequirePermissions(DefinedPermissions.Conference.CanKickParticipant)
                .ConferenceMustBeOpen().Send();
        }

        public Task<SuccessOrError<ParticipantPermissionResponse>> FetchPermissions(string? targetParticipantId)
        {
            var (conferenceId, myParticipantId) = GetContextParticipant();
            var fetchPermissionsOfParticipantId = targetParticipantId ?? myParticipantId;

            var requiredPermissions = new List<PermissionDescriptor<bool>>();
            if (fetchPermissionsOfParticipantId != myParticipantId)
                requiredPermissions.Add(DefinedPermissions.Permissions.CanSeeAnyParticipantsPermissions);

            return GetInvoker()
                .Create(new FetchPermissionsRequest(new Participant(conferenceId, fetchPermissionsOfParticipantId)))
                .ConferenceMustBeOpen().RequirePermissions(requiredPermissions).Send();
        }

        public Task<SuccessOrError<Unit>> SetTemporaryPermission(SetTemporaryPermissionDto dto)
        {
            var (conferenceId, _) = GetContextParticipant();

            return GetInvoker()
                .Create(new SetTemporaryPermissionRequest(new Participant(conferenceId, dto.ParticipantId),
                    dto.PermissionKey, dto.Value))
                .RequirePermissions(DefinedPermissions.Permissions.CanGiveTemporaryPermission).ValidateObject(dto)
                .ConferenceMustBeOpen().Send();
        }

        public Task<SuccessOrError<IReadOnlyList<Room>>> CreateRooms(IReadOnlyList<RoomCreationInfo> dto)
        {
            var (conferenceId, _) = GetContextParticipant();

            return GetInvoker().Create(new CreateRoomsRequest(conferenceId, dto))
                .RequirePermissions(DefinedPermissions.Rooms.CanCreateAndRemove).ValidateObject(dto)
                .ConferenceMustBeOpen().Send();
        }

        public Task<SuccessOrError<Unit>> RemoveRooms(IReadOnlyList<string> dto)
        {
            var (conferenceId, _) = GetContextParticipant();

            return GetInvoker().Create(new RemoveRoomsRequest(conferenceId, dto))
                .RequirePermissions(DefinedPermissions.Rooms.CanCreateAndRemove).ConferenceMustBeOpen().Send();
        }

        public Task<SuccessOrError<Unit>> SwitchRoom(SwitchRoomDto dto)
        {
            var participant = GetContextParticipant();

            return GetInvoker().Create(new SetParticipantRoomRequest(participant, dto.RoomId))
                .RequirePermissions(DefinedPermissions.Rooms.CanSwitchRoom).ValidateObject(dto).ConferenceMustBeOpen()
                .Send();
        }

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

        public async Task<SuccessOrError<Unit>> SendChatMessage(SendChatMessageDto dto)
        {
            if (!ChatValidationExtensions.TryParseChatChannel(dto.Channel, out var channel))
                return new FieldValidationError(nameof(FetchChatMessagesDto.Channel), "Could not parse chat channel");

            var participant = GetContextParticipant();
            var builder = GetInvoker()
                .Create(new SendChatMessageRequest(participant, dto.Message, channel, dto.Options))
                .VerifyCanSendToChatChannel(channel).VerifyMessageConformsOptions(dto, channel).ValidateObject(dto)
                .ConferenceMustBeOpen();

            return await builder.Send();
        }

        public async Task<SuccessOrError<IReadOnlyList<ChatMessageDto>>> RequestChat(FetchChatMessagesDto dto)
        {
            if (!ChatValidationExtensions.TryParseChatChannel(dto.Channel, out var channel))
                return new FieldValidationError(nameof(FetchChatMessagesDto.Channel), "Could not parse chat channel");

            var participant = GetContextParticipant();
            var result = await GetInvoker()
                .Create(new FetchMessagesRequest(participant.ConferenceId, channel, dto.Start, dto.End))
                .VerifyCanSendToChatChannel(channel).Send();

            if (!result.Success) return result.Error;

            var (messages, totalMessages) = result.Response;
            var (start, _) = IndexUtils.TranslateStartEndIndex(dto.Start, dto.End, totalMessages);

            var messageDtos = new List<ChatMessageDto>(messages.Count);
            var currentId = start;
            foreach (var chatMessage in messages)
            {
                ChatMessageSender? sender = null;
                if (!chatMessage.Options.IsAnonymous)
                    sender = chatMessage.Sender;

                messageDtos.Add(new ChatMessageDto(currentId++, dto.Channel, sender, chatMessage.Message,
                    chatMessage.Timestamp, chatMessage.Options));
            }

            return messageDtos;
        }

        public async Task<SuccessOrError<Unit>> SetUserIsTyping(SetUserTypingDto dto)
        {
            if (!ChatValidationExtensions.TryParseChatChannel(dto.Channel, out var channel))
                return new FieldValidationError(nameof(FetchChatMessagesDto.Channel), "Could not parse chat channel");

            var participant = GetContextParticipant();
            return await GetInvoker().Create(new SetParticipantTypingRequest(participant, channel, dto.IsTyping))
                .VerifyCanSendToChatChannel(channel).ConferenceMustBeOpen().Send();
        }

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
