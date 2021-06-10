using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Strive.Core;
using Strive.Core.Errors;
using Strive.Core.Extensions;
using Strive.Core.Interfaces;
using Strive.Core.Services;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.BreakoutRooms.Requests;
using Strive.Core.Services.Chat;
using Strive.Core.Services.Chat.Requests;
using Strive.Core.Services.ConferenceControl;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.ConferenceControl.Requests;
using Strive.Core.Services.Equipment.Requests;
using Strive.Core.Services.Media;
using Strive.Core.Services.Media.Requests;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Permissions.Requests;
using Strive.Core.Services.Permissions.Responses;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Rooms.Requests;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Requests;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Synchronization.Extensions;
using Strive.Core.Utilities;
using Strive.Extensions;
using Strive.Hubs.Core.Dtos;
using Strive.Hubs.Core.Responses;
using Strive.Hubs.Core.Services;
using Strive.Hubs.Core.Services.Middlewares;
using Strive.Hubs.Core.Validators.Extensions;
using Strive.Infrastructure.Extensions;
using Strive.Messaging.SFU.Dto;

namespace Strive.Hubs.Core
{
    [Authorize]
    public class CoreHub : ScopedHub, ISfuConnectionHub
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
                _logger.LogDebug("Client {connectionId} tries to connect", Context.ConnectionId);

                try
                {
                    await HandleJoin();
                }
                catch (Exception e)
                {
                    var error = e.ToError();
                    _logger.LogWarning("Client join was not successful: {@error}", error);

                    await Clients.Caller.SendAsync(CoreHubMessages.OnConnectionError, error);
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

        private async ValueTask<string?> GetParticipantRoomId(Participant participant)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(participant.ConferenceId,
                SynchronizedRooms.SyncObjId);
            rooms.Participants.TryGetValue(participant.Id, out var roomId);
            return roomId;
        }

        private ParticipantMetadata GetMetadata()
        {
            var httpContext = GetHttpContext();
            var name = httpContext.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? string.Empty;

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

            var repo = HubScope.Resolve<IJoinedParticipantsRepository>();

            await using var @lock = await repo.LockParticipantJoin(participant);

            if (_connections.TryRemoveParticipant(participant.Id,
                new ParticipantConnection(participant.ConferenceId, connectionId)))
            {
                _logger.LogDebug("Participant connection exists, publish ParticipantLeftNotification");
                await _mediator.Publish(new ParticipantLeftNotification(participant, connectionId),
                    @lock.HandleLostToken);
            }
            else
            {
                _logger.LogDebug("OnDisconnectedAsync() | Connection does not exist, dont publish notification");
            }
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
            if (dto.Value?.Type == JTokenType.Null)
                dto = dto with {Value = null};

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

            return GetInvoker().Create(SetParticipantRoomRequest.MoveParticipant(participant, dto.RoomId))
                .RequirePermissions(DefinedPermissions.Rooms.CanSwitchRoom).ValidateObject(dto).ConferenceMustBeOpen()
                .Send();
        }

        public Task<SuccessOrError<Unit>> OpenBreakoutRooms(OpenBreakoutRoomsDto request)
        {
            var (conferenceId, _) = GetContextParticipant();

            return GetInvoker()
                .Create(new OpenBreakoutRoomsRequest(request.Amount, request.Deadline, request.Description,
                    request.AssignedRooms, conferenceId))
                .RequirePermissions(DefinedPermissions.Rooms.CanCreateAndRemove).ConferenceMustBeOpen().Send();
        }

        public Task<SuccessOrError<Unit>> CloseBreakoutRooms()
        {
            var (conferenceId, _) = GetContextParticipant();

            return GetInvoker().Create(new CloseBreakoutRoomsRequest(conferenceId))
                .RequirePermissions(DefinedPermissions.Rooms.CanCreateAndRemove).ConferenceMustBeOpen().Send();
        }

        public Task<SuccessOrError<Unit>> ChangeBreakoutRooms(JsonPatchDocument<BreakoutRoomsConfig> dto)
        {
            var (conferenceId, _) = GetContextParticipant();

            return GetInvoker().Create(new ChangeBreakoutRoomsRequest(conferenceId, dto))
                .RequirePermissions(DefinedPermissions.Rooms.CanCreateAndRemove).ConferenceMustBeOpen().Send();
        }

        public async Task<SuccessOrError<Unit>> SendChatMessage(SendChatMessageDto dto)
        {
            if (!ChatValidationExtensions.TryParseChatChannel(dto.Channel, out var channel))
                return new FieldValidationError(nameof(FetchChatMessagesDto.Channel), "Could not parse chat channel");

            var participant = GetContextParticipant();
            var builder = GetInvoker()
                .Create(new SendChatMessageRequest(participant, dto.Message, channel, dto.Options))
                .VerifyCanSendToChatChannel(channel).VerifyMessageConformsOptions(dto).ValidateObject(dto)
                .ConferenceMustBeOpen();

            return await builder.Send();
        }

        public async Task<SuccessOrError<IReadOnlyList<ChatMessageDto>>> FetchChatMessages(FetchChatMessagesDto dto)
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

        public async Task<SuccessOrError<SfuConnectionInfo>> FetchSfuConnectionInfo()
        {
            var participant = GetContextParticipant();
            return await GetInvoker().Create(new FetchSfuConnectionInfoRequest(participant, Context.ConnectionId))
                .Send();
        }

        public async Task<SuccessOrError<Unit>> ChangeParticipantProducer(ChangeParticipantProducerDto dto)
        {
            var (conferenceId, _) = GetContextParticipant();

            return await GetInvoker()
                .Create(new ChangeParticipantProducerRequest(new Participant(conferenceId, dto.ParticipantId),
                    dto.Source, dto.Action))
                .RequirePermissions(DefinedPermissions.Media.CanChangeOtherParticipantsProducers).Send();
        }

        public async Task<SuccessOrError<string>> GetEquipmentToken()
        {
            var participant = GetContextParticipant();
            return await GetInvoker().Create(new FetchEquipmentTokenRequest(participant)).Send();
        }

        public async Task<SuccessOrError<Unit>> SendEquipmentCommand(SendEquipmentCommandDto dto)
        {
            var participant = GetContextParticipant();
            return await GetInvoker()
                .Create(new SendEquipmentCommandRequest(participant, dto.ConnectionId, dto.Source, dto.DeviceId,
                    dto.Action)).ValidateObject(dto).Send();
        }

        public async Task<SuccessOrError<Unit>> SetScene(IScene? scene)
        {
            if (scene == null)
                return SuccessOrError<Unit>.Failed(new FieldValidationError("scene", "Scene must not be null"));

            var participant = GetContextParticipant();
            var roomId = await GetParticipantRoomId(participant);
            if (roomId == null)
                return SuccessOrError<Unit>.Failed(SceneError.RoomNotFound);

            return await GetInvoker().Create(new SetSceneRequest(participant.ConferenceId, roomId, scene))
                .RequirePermissions(DefinedPermissions.Scenes.CanSetScene).Send();
        }

        public async Task<SuccessOrError<Unit>> SetOverwrittenScene(IScene? scene)
        {
            var participant = GetContextParticipant();
            var roomId = await GetParticipantRoomId(participant);
            if (roomId == null)
                return SuccessOrError<Unit>.Failed(SceneError.RoomNotFound);

            return await GetInvoker()
                .Create(new SetOverwrittenContentSceneRequest(participant.ConferenceId, roomId, scene))
                .RequirePermissions(DefinedPermissions.Scenes.CanOverwriteContentScene).Send();
        }

        public async Task<SuccessOrError<Unit>> TalkingStickEnqueue()
        {
            var participant = GetContextParticipant();

            return await GetInvoker().Create(new TalkingStickEnqueueRequest(participant, false))
                .RequirePermissions(DefinedPermissions.Scenes.CanQueueForTalkingStick).Send();
        }

        public async Task<SuccessOrError<Unit>> TalkingStickDequeue()
        {
            var participant = GetContextParticipant();

            return await GetInvoker().Create(new TalkingStickEnqueueRequest(participant, true)).Send();
        }

        public async Task<SuccessOrError<Unit>> TalkingStickTake()
        {
            var participant = GetContextParticipant();
            var roomId = await GetParticipantRoomId(participant);
            if (roomId == null)
                return SuccessOrError<Unit>.Failed(SceneError.RoomNotFound);

            return await GetInvoker().Create(new TalkingStickPassRequest(participant, roomId, false))
                .RequirePermissions(DefinedPermissions.Scenes.CanTakeTalkingStick).Send();
        }

        public async Task<SuccessOrError<Unit>> TalkingStickPass(string? participantId)
        {
            if (participantId == null)
                return SuccessOrError<Unit>.Failed(new FieldValidationError(nameof(participantId),
                    "Participant id not be null"));

            var participant = GetContextParticipant();
            var roomId = await GetParticipantRoomId(participant);
            if (roomId == null)
                return SuccessOrError<Unit>.Failed(SceneError.RoomNotFound);

            return await GetInvoker()
                .Create(new TalkingStickPassRequest(new Participant(participant.ConferenceId, participantId), roomId,
                    false)).RequirePermissions(DefinedPermissions.Scenes.CanPassTalkingStick).Send();
        }

        public async Task<SuccessOrError<Unit>> TalkingStickReturn()
        {
            var participant = GetContextParticipant();
            return await GetInvoker().Create(new TalkingStickReturnRequest(participant)).Send();
        }

        public async Task<SuccessOrError<string>> CreatePoll(CreatePollDto dto)
        {
            var participant = GetContextParticipant();
            return await GetInvoker().Create(new CreatePollRequest(participant.ConferenceId, dto.Instruction,
                    dto.Config, dto.InitialState, dto.RoomId)).RequirePermissions(DefinedPermissions.Poll.CanOpenPoll)
                .ValidateObject(dto).Send();
        }

        public async Task<SuccessOrError<Unit>> SubmitPollAnswer(SubmitPollAnswerDto dto)
        {
            var participant = GetContextParticipant();
            return await GetInvoker().Create(new SubmitAnswerRequest(participant, dto.PollId, dto.Answer))
                .ValidateObject(dto).Send();
        }

        public async Task<SuccessOrError<Unit>> UpdatePollState(UpdatePollStateDto dto)
        {
            var participant = GetContextParticipant();
            return await GetInvoker()
                .Create(new UpdatePollStateRequest(participant.ConferenceId, dto.PollId, dto.State))
                .RequirePermissions(DefinedPermissions.Poll.CanOpenPoll).ValidateObject(dto).Send();
        }

        public async Task<SuccessOrError<Unit>> DeletePoll(DeletePollDto dto)
        {
            var participant = GetContextParticipant();
            return await GetInvoker().Create(new DeletePollRequest(participant.ConferenceId, dto.PollId))
                .RequirePermissions(DefinedPermissions.Poll.CanOpenPoll).ValidateObject(dto).Send();
        }

        public async Task<SuccessOrError<Unit>> DeletePollAnswer(DeletePollAnswerDto dto)
        {
            var participant = GetContextParticipant();
            return await GetInvoker().Create(new SubmitAnswerRequest(participant, dto.PollId, null)).ValidateObject(dto)
                .Send();
        }
    }
}
