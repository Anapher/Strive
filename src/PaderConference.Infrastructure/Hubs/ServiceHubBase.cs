using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Services;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure.Hubs
{
    public abstract class ServiceHubBase : HubBase
    {
        protected readonly IConferenceManager ConferenceManager;
        protected readonly IEnumerable<IConferenceServiceManager> ConferenceServices;

        protected ServiceHubBase(IConnectionMapping connectionMapping, IConferenceManager conferenceManager,
            IEnumerable<IConferenceServiceManager> conferenceServices,
            ILogger logger) : base(connectionMapping, logger)
        {
            ConferenceManager = conferenceManager;
            ConferenceServices = conferenceServices;
        }

        protected ValueTask<T> GetConferenceService<T>(Participant participant) where T : IConferenceService
        {
            var conferenceId = ConferenceManager.GetConferenceOfParticipant(participant);

            return ConferenceServices.OfType<IConferenceServiceManager<T>>().First().GetService(conferenceId);
        }

        protected async Task<bool> AssertConference(IServiceMessage message, MethodOptions options)
        {
            if (options.ConferenceCanBeClosed) return true;

            var conferenceId = ConferenceManager.GetConferenceOfParticipant(message.Participant);

            if (!await ConferenceManager.GetIsConferenceOpen(conferenceId))
            {
                await message.ResponseError(ConferenceError.NotOpen);
                return false;
            }

            return true;
        }

        protected async Task InvokeService<TService, T>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask>> action, MethodOptions options = default)
            where TService : IConferenceService
        {
            if (GetMessage(dto, out var message) && await AssertConference(message, options))
            {
                var service = await GetConferenceService<TService>(message.Participant);
                var method = action(service);
                await method(message);
            }
        }

        protected async Task InvokeService<TService>(Func<TService, Func<IServiceMessage, ValueTask>> action,
            MethodOptions options = default)
            where TService : IConferenceService
        {
            if (GetMessage(out var message) && await AssertConference(message, options))
            {
                var service = await GetConferenceService<TService>(message.Participant);
                var method = action(service);
                await method(message);
            }
        }

        protected async Task<TResult> InvokeService<TService, T, TResult>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<TResult>>> action, MethodOptions options = default)
            where TService : IConferenceService
        {
            if (GetMessage(dto, out var message) && await AssertConference(message, options))
            {
                var service = await GetConferenceService<TService>(message.Participant);
                var method = action(service);
                return await method(message);
            }

            return default!;
        }

        protected async Task<TResult> InvokeService<TService, TResult>(
            Func<TService, Func<IServiceMessage, ValueTask<TResult>>> action, MethodOptions options = default)
            where TService : IConferenceService
        {
            if (GetMessage(out var message) && await AssertConference(message, options))
            {
                var service = await GetConferenceService<TService>(message.Participant);
                var method = action(service);
                return await method(message);
            }

            return default!;
        }
    }
}