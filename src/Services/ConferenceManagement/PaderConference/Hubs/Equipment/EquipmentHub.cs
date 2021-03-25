// https://github.com/Anapher/PaderConference/blob/master/src/PaderConference.Core/Services/Equipment/EquipmentService.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Dto;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Equipment;
using PaderConference.Core.Services.Equipment.Notifications;
using PaderConference.Core.Services.Equipment.Requests;
using PaderConference.Core.Services.Media;
using PaderConference.Core.Services.Media.Requests;
using PaderConference.Extensions;
using PaderConference.Hubs.Equipment.Dtos;

namespace PaderConference.Hubs.Equipment
{
    [AllowAnonymous]
    public class EquipmentHub : ScopedHub
    {
        private readonly ILogger<EquipmentHub> _logger;
        private readonly IMediator _mediator;

        public EquipmentHub(ILifetimeScope scope) : base(scope)
        {
            _logger = scope.Resolve<ILogger<EquipmentHub>>();
            _mediator = HubScope.Resolve<IMediator>();
        }

        public override async Task OnConnectedAsync()
        {
            using (_logger.BeginMethodScope(new Dictionary<string, object> {{"connectionId", Context.ConnectionId}}))
            {
                _logger.LogDebug("Equipment tries to connect");

                try
                {
                    await HandleJoin();
                }
                catch (Exception e)
                {
                    var error = e.ToError();
                    _logger.LogWarning("Equipment join was not successful: {@error}", error);

                    await Clients.Caller.SendAsync(EquipmentHubMessages.OnConnectionError, error);
                    Context.Abort();
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogDebug("Equipment {connectionId} disconnected.", Context.ConnectionId);

            var (participant, _) = GetContextParticipantWithToken();

            await _mediator.Publish(new EquipmentDisconnectedNotification(participant, Context.ConnectionId));
        }

        private async Task HandleJoin()
        {
            var (participant, token) = GetContextParticipantWithToken();

            await _mediator.Send(new AuthenticateEquipmentRequest(participant, token), Context.ConnectionAborted);
            await Groups.AddToGroupAsync(Context.ConnectionId, EquipmentGroups.OfParticipant(participant));
        }

        private (Participant, string) GetContextParticipantWithToken()
        {
            var httpContext = this.GetHttpContext();

            var conferenceId = httpContext.Request.Query["conferenceId"].ToString();
            var participantId = httpContext.Request.Query["participantId"].ToString();
            var token = httpContext.Request.Query["token"].ToString();

            return (new Participant(conferenceId, participantId), token);
        }

        public async Task<SuccessOrError<Unit>> Initialize(InitializeEquipmentDto dto)
        {
            try
            {
                var (participant, _) = GetContextParticipantWithToken();

                await _mediator.Send(new InitializeEquipmentRequest(participant, Context.ConnectionId, dto.Name,
                    dto.Devices));

                return SuccessOrError<Unit>.Succeeded(Unit.Value);
            }
            catch (Exception e)
            {
                return e.ToError();
            }
        }

        public async Task<SuccessOrError<Unit>> UpdateStatus(Dictionary<string, UseMediaStateInfo> dto)
        {
            try
            {
                var (participant, _) = GetContextParticipantWithToken();

                await _mediator.Send(new UpdateStatusRequest(participant, Context.ConnectionId, dto));
                return SuccessOrError<Unit>.Succeeded(Unit.Value);
            }
            catch (Exception e)
            {
                return e.ToError();
            }
        }

        public async Task<SuccessOrError<Unit>> ErrorOccurred(Error dto)
        {
            try
            {
                var (participant, _) = GetContextParticipantWithToken();

                await _mediator.Publish(new EquipmentErrorNotification(participant, Context.ConnectionId, dto));
                return SuccessOrError<Unit>.Succeeded(Unit.Value);
            }
            catch (Exception e)
            {
                return e.ToError();
            }
        }

        public async Task<SuccessOrError<SfuConnectionInfo>> FetchSfuConnectionInfo()
        {
            try
            {
                var (participant, _) = GetContextParticipantWithToken();
                var result = await _mediator.Send(new FetchSfuConnectionInfoRequest(participant, Context.ConnectionId));
                return new SuccessOrError<SfuConnectionInfo>(result);
            }
            catch (Exception e)
            {
                return e.ToError();
            }
        }
    }
}
