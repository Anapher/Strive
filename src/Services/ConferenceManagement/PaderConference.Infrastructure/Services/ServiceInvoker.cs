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

namespace PaderConference.Infrastructure.Services
{
    public class ServiceInvoker : IServiceInvoker
    {
        private readonly IConnectionMapping _connectionMapping;
        private readonly IComponentContext _componentContext;
        private readonly IConferenceManager _conferenceManager;
        private readonly IEnumerable<IConferenceServiceManager> _conferenceServices;
        private readonly IServiceInvokerContext _context;
        private readonly ILogger<ServiceInvoker> _logger;

        public ServiceInvoker(IConnectionMapping connectionMapping, IComponentContext componentContext,
            IConferenceManager conferenceManager, IEnumerable<IConferenceServiceManager> conferenceServices,
            IServiceInvokerContext context, ILogger<ServiceInvoker> logger)
        {
            _connectionMapping = connectionMapping;
            _componentContext = componentContext;
            _conferenceManager = conferenceManager;
            _conferenceServices = conferenceServices;
            _context = context;
            _logger = logger;
        }

        public async Task<SuccessOrError> InvokeService<TService, T>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<SuccessOrError>>> action, MethodOptions options = default)
            where TService : IConferenceService
        {
            if (!AssertParticipant(_context.ConnectionId, out var participant))
                return SuccessOrError.Failed(ConferenceError.ParticipantNotRegistered);

            if (!ValidateDto(dto, out var validationError))
                return validationError;

            var message = _context.CreateMessage(dto, participant);

            var result = await AssertConference(message, options);
            if (!result.Success)
                return result;

            var service = await GetConferenceService<TService>(message.Participant);
            var method = action(service);

            return await WrapServiceCall(method, message);
        }

        public async Task<SuccessOrError> InvokeService<TService>(
            Func<TService, Func<IServiceMessage, ValueTask<SuccessOrError>>> action, MethodOptions options = default)
            where TService : IConferenceService
        {
            if (!AssertParticipant(_context.ConnectionId, out var participant))
                return SuccessOrError.Failed(ConferenceError.ParticipantNotRegistered);

            var message = _context.CreateMessage(participant);

            var result = await AssertConference(message, options);
            if (!result.Success)
                return result;

            var service = await GetConferenceService<TService>(message.Participant);
            var method = action(service);

            return await WrapServiceCall(method, message);
        }

        public async Task<SuccessOrError<TResult>> InvokeService<TService, T, TResult>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<SuccessOrError<TResult>>>> action,
            MethodOptions options = default) where TService : IConferenceService
        {
            if (!AssertParticipant(_context.ConnectionId, out var participant))
                return SuccessOrError<TResult>.Failed(ConferenceError.ParticipantNotRegistered);

            if (!ValidateDto(dto, out var validationError))
                return validationError;

            var message = _context.CreateMessage(dto, participant);

            var result = await AssertConference(message, options);
            if (!result.Success)
                return result.Error;

            var service = await GetConferenceService<TService>(message.Participant);
            var method = action(service);

            return await WrapServiceCall(method, message);
        }

        public async Task<SuccessOrError<TResult>> InvokeService<TService, TResult>(
            Func<TService, Func<IServiceMessage, ValueTask<SuccessOrError<TResult>>>> action,
            MethodOptions options = default) where TService : IConferenceService
        {
            if (!AssertParticipant(_context.ConnectionId, out var participant))
                return SuccessOrError<TResult>.Failed(ConferenceError.ParticipantNotRegistered);

            var message = _context.CreateMessage(participant);

            var result = await AssertConference(message, options);
            if (!result.Success)
                return result.Error;

            var service = await GetConferenceService<TService>(message.Participant);
            var method = action(service);

            return await WrapServiceCall(method, message);
        }

        private bool AssertParticipant(string connectionId, [NotNullWhen(true)] out Participant? participant)
        {
            if (!_connectionMapping.Connections.TryGetValue(connectionId, out participant))
            {
                _logger.LogWarning("Connection {connectionId} is not mapped to a participant.", connectionId);
                return false;
            }

            return true;
        }

        private ValueTask<T> GetConferenceService<T>(Participant participant) where T : IConferenceService
        {
            var conferenceId = _conferenceManager.GetConferenceOfParticipant(participant);

            return _conferenceServices.OfType<IConferenceServiceManager<T>>().First().GetService(conferenceId);
        }

        private async Task<SuccessOrError> WrapServiceCall<TInput>(Func<TInput, ValueTask<SuccessOrError>> func,
            TInput param) where TInput : IServiceMessage
        {
            using var _ = _logger.BeginScope(param.GetScopeData());

            try
            {
                var result = await func(param);
                if (!result.Success)
                    _logger.LogWarning("Executing {func} failed with error: {@error}", func, result.Error);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred on executing {func} with param {@param}", func, param);
                return CommonError.InternalServiceError("An error occurred on invoking service method.");
            }
        }

        private async Task<SuccessOrError<TReturn>> WrapServiceCall<TReturn, TInput>(
            Func<TInput, ValueTask<SuccessOrError<TReturn>>> func, TInput param) where TInput : IServiceMessage
        {
            using var _ = _logger.BeginScope(param.GetScopeData());

            try
            {
                var result = await func(param);
                if (!result.Success)
                    _logger.LogWarning("Executing {func} failed with error: {@error}", func, result.Error);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred on executing {func} with param {@param}", func, param);
                return CommonError.InternalServiceError("An error occurred on invoking service method.");
            }
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

        protected async Task<SuccessOrError> AssertConference(IServiceMessage message, MethodOptions options)
        {
            if (options.ConferenceCanBeClosed) return SuccessOrError.Succeeded;

            var conferenceId = _conferenceManager.GetConferenceOfParticipant(message.Participant);
            if (!await _conferenceManager.GetIsConferenceOpen(conferenceId))
                return ConferenceError.ConferenceNotOpen;

            return SuccessOrError.Succeeded;
        }
    }
}
