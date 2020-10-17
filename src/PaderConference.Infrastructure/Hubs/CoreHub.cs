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
using PaderConference.Infrastructure.Services.Media;
using PaderConference.Infrastructure.Services.Media.Communication;
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
                            participant = await _conferenceManager.Participate(conferenceId, userId, role,
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

                        await Groups.AddToGroupAsync(Context.ConnectionId, conferenceId);

                        // initialize all services before submitting events
                        var services = _conferenceServices.Select(x => x.GetService(conferenceId, _conferenceServices))
                            .ToList();
                        foreach (var valueTask in services) await valueTask;

                        foreach (var service in services)
                            await service.Result.OnClientConnected(participant);
                    }
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Todo: close conference if it was the last participant and some time passed
            if (!_connectionMapping.Connections.TryGetValue(Context.ConnectionId, out var participant))
                return;

            var conferenceId = _conferenceManager.GetConferenceOfParticipant(participant);

            _connectionMapping.Remove(Context.ConnectionId);
            await _conferenceManager.RemoveParticipant(participant);

            foreach (var service in _conferenceServices)
                await (await service.GetService(conferenceId, _conferenceServices))
                    .OnClientDisconnected(participant, Context.ConnectionId);
        }

        private ValueTask<T> GetConferenceService<T>(Participant participant) where T : IConferenceService
        {
            var conferenceId = _conferenceManager.GetConferenceOfParticipant(participant);

            return _conferenceServices.OfType<IConferenceServiceManager<T>>().First()
                .GetService(conferenceId, _conferenceServices);
        }

        private async Task InvokeService<TService, T>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask>> action) where TService : IConferenceService
        {
            if (GetMessage(dto, out var message))
            {
                var service = await GetConferenceService<TService>(message.Participant);
                var method = action(service);
                await method(message);
            }
        }

        private async Task InvokeService<TService>(Func<TService, Func<IServiceMessage, ValueTask>> action)
            where TService : IConferenceService
        {
            if (GetMessage(out var message))
            {
                var service = await GetConferenceService<TService>(message.Participant);
                var method = action(service);
                await method(message);
            }
        }

        private async Task<TResult> InvokeService<TService, T, TResult>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<TResult>>> action) where TService : IConferenceService
        {
            if (GetMessage(dto, out var message))
            {
                var service = await GetConferenceService<TService>(message.Participant);
                var method = action(service);
                return await method(message);
            }

            return default!;
        }

        private async Task<TResult> InvokeService<TService, TResult>(
            Func<TService, Func<IServiceMessage, ValueTask<TResult>>> action) where TService : IConferenceService
        {
            if (GetMessage(out var message))
            {
                var service = await GetConferenceService<TService>(message.Participant);
                var method = action(service);
                return await method(message);
            }

            return default!;
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
            return InvokeService<MediaService, JsonElement, JsonElement?>(element, service =>
                service.Redirect(RedisCommunication.Request.CreateTransport));
        }

        public Task<JsonElement?> ConnectWebRtcTransport(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element, service =>
                service.Redirect(RedisCommunication.Request.ConnectTransport));
        }

        public Task<JsonElement?> ProduceWebRtcTransport(JsonElement element)
        {
            return InvokeService<MediaService, JsonElement, JsonElement?>(element, service =>
                service.Redirect(RedisCommunication.Request.TransportProduce));
        }
    }
}