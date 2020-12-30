using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PaderConference.Core;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;

namespace PaderConference.Infrastructure.Hubs
{
    public abstract class ServiceHubBase : HubBase
    {
        protected readonly IConferenceManager ConferenceManager;
        protected readonly IEnumerable<IConferenceServiceManager> ConferenceServices;
        private readonly IComponentContext _componentContext;

        protected ServiceHubBase(IConnectionMapping connectionMapping, IConferenceManager conferenceManager,
            IEnumerable<IConferenceServiceManager> conferenceServices, IComponentContext componentContext,
            ILogger logger) : base(connectionMapping, logger)
        {
            ConferenceManager = conferenceManager;
            ConferenceServices = conferenceServices;
            _componentContext = componentContext;
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
                return ConferenceError.ConferenceNotOpen;

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
                return CommonError.InternalServiceError("An error occurred on invoking service method.");
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
                return CommonError.InternalServiceError("An error occurred on invoking service method.");
            }
        }

        protected async Task<SuccessOrError> InvokeService<TService, T>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<SuccessOrError>>> action, MethodOptions options = default)
            where TService : IConferenceService
        {
            if (!AssertParticipant(out var participant))
                return SuccessOrError.Failed(ConferenceError.ParticipantNotRegistered);

            if (!ValidateDto(dto, out var validationError))
                return validationError;

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

            if (!ValidateDto(dto, out var validationError))
                return validationError;

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

        private bool ValidateDto<T>(T dto, [NotNullWhen(false)] out Error? error)
        {
            if (!_componentContext.TryResolve<IValidator<T>>(out var validator))
            {
                error = null;
                return true;
            }

            var result = validator.Validate(dto);
            if (result.IsValid)
            {
                error = null;
                return true;
            }

            error = result.ToError();
            return false;
        }
    }
}
