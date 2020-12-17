using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;

namespace PaderConference.Infrastructure.Hubs
{
    public abstract class ServiceHubBase : HubBase
    {
        protected readonly IConferenceManager ConferenceManager;
        protected readonly IEnumerable<IConferenceServiceManager> ConferenceServices;

        protected ServiceHubBase(IConnectionMapping connectionMapping, IConferenceManager conferenceManager,
            IEnumerable<IConferenceServiceManager> conferenceServices, ILogger logger) : base(connectionMapping, logger)
        {
            ConferenceManager = conferenceManager;
            ConferenceServices = conferenceServices;
        }

        protected ValueTask<T> GetConferenceService<T>(Participant participant) where T : IConferenceService
        {
            var conferenceId = ConferenceManager.GetConferenceOfParticipant(participant);

            return ConferenceServices.OfType<IConferenceServiceManager<T>>().First().GetService(conferenceId);
        }

        protected async Task<SuccessOrError> AssertConference(IServiceMessage message, MethodOptions options)
        {
            if (options.ConferenceCanBeClosed) return SuccessOrError.Succeeded;

            var conferenceId = ConferenceManager.GetConferenceOfParticipant(message.Participant);
            if (!await ConferenceManager.GetIsConferenceOpen(conferenceId))
                return ConferenceError.NotOpen;

            return SuccessOrError.Succeeded;
        }

        private async Task<SuccessOrError> WrapServiceCall<TInput>(Func<TInput, ValueTask<SuccessOrError>> func,
            TInput param) where TInput : IServiceMessage
        {
            using var _ = Logger.BeginScope(param.GetScopeData());

            try
            {
                var result = await func(param);
                if (!result.Success)
                    Logger.LogWarning("Executing {func} failed with error: {@error}", func, result.Error);

                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An error occurred on executing {func} with param {@param}", func, param);
                return ConferenceError.InternalServiceError;
            }
        }

        private async Task<SuccessOrError<TReturn>> WrapServiceCall<TReturn, TInput>(
            Func<TInput, ValueTask<SuccessOrError<TReturn>>> func, TInput param) where TInput : IServiceMessage
        {
            using var _ = Logger.BeginScope(param.GetScopeData());

            try
            {
                var result = await func(param);
                if (!result.Success)
                    Logger.LogWarning("Executing {func} failed with error: {@error}", func, result.Error);

                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An error occurred on executing {func} with param {@param}", func, param);
                return ConferenceError.InternalServiceError;
            }
        }

        protected async Task<SuccessOrError> InvokeService<TService, T>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<SuccessOrError>>> action, MethodOptions options = default)
            where TService : IConferenceService
        {
            if (!AssertParticipant(out var participant))
                return SuccessOrError.Failed(ConferenceError.ParticipantNotRegistered);

            var message = CreateMessage(dto, participant);

            var result = await AssertConference(message, options);
            if (!result.Success)
                return result;

            var service = await GetConferenceService<TService>(message.Participant);
            var method = action(service);

            return await WrapServiceCall(method, message);
        }

        protected async Task<SuccessOrError> InvokeService<TService>(
            Func<TService, Func<IServiceMessage, ValueTask<SuccessOrError>>> action, MethodOptions options = default)
            where TService : IConferenceService
        {
            if (!AssertParticipant(out var participant))
                return SuccessOrError.Failed(ConferenceError.ParticipantNotRegistered);

            var message = CreateMessage(participant);

            var result = await AssertConference(message, options);
            if (!result.Success)
                return result;

            var service = await GetConferenceService<TService>(message.Participant);
            var method = action(service);

            return await WrapServiceCall(method, message);
        }

        protected async Task<SuccessOrError<TResult>> InvokeService<TService, T, TResult>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<SuccessOrError<TResult>>>> action,
            MethodOptions options = default) where TService : IConferenceService
        {
            if (!AssertParticipant(out var participant))
                return SuccessOrError<TResult>.Failed(ConferenceError.ParticipantNotRegistered);

            var message = CreateMessage(dto, participant);

            var result = await AssertConference(message, options);
            if (!result.Success)
                return result.Error;

            var service = await GetConferenceService<TService>(message.Participant);
            var method = action(service);

            return await WrapServiceCall(method, message);
        }

        protected async Task<SuccessOrError<TResult>> InvokeService<TService, TResult>(
            Func<TService, Func<IServiceMessage, ValueTask<SuccessOrError<TResult>>>> action,
            MethodOptions options = default) where TService : IConferenceService
        {
            if (!AssertParticipant(out var participant))
                return SuccessOrError<TResult>.Failed(ConferenceError.ParticipantNotRegistered);

            var message = CreateMessage(participant);

            var result = await AssertConference(message, options);
            if (!result.Success)
                return result.Error;

            var service = await GetConferenceService<TService>(message.Participant);
            var method = action(service);

            return await WrapServiceCall(method, message);
        }
    }
}
