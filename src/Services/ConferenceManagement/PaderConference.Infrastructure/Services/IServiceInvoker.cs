using System;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Infrastructure.Hubs;

namespace PaderConference.Infrastructure.Services
{
    /// <summary>
    ///     Commander class for invoking service methods. The class provides methods for validating a participant, validating
    ///     the parameters, handling errors, etc.
    /// </summary>
    public interface IServiceInvoker
    {
        /// <summary>
        ///     Invoke a method using a dto and no response value
        /// </summary>
        /// <typeparam name="TService">The type of the service</typeparam>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <param name="dto">The parameter</param>
        /// <param name="action">The action that should be called on the service</param>
        /// <param name="options">Optional options for this call</param>
        /// <returns>Return the result of this action</returns>
        Task<SuccessOrError> InvokeService<TService, T>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<SuccessOrError>>> action, MethodOptions options = default)
            where TService : IConferenceService;

        /// <summary>
        ///     Invoke a method without a parameter and with no response value
        /// </summary>
        /// <typeparam name="TService">The type of the service</typeparam>
        /// <param name="action">The action that should be called on the service</param>
        /// <param name="options">Optional options for this call</param>
        /// <returns>Return the result of this action</returns>
        Task<SuccessOrError> InvokeService<TService>(
            Func<TService, Func<IServiceMessage, ValueTask<SuccessOrError>>> action, MethodOptions options = default)
            where TService : IConferenceService;

        /// <summary>
        ///     Invoke a method using a dto and a response value
        /// </summary>
        /// <typeparam name="TService">The type of the service</typeparam>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <typeparam name="TResult">The type of the response value</typeparam>
        /// <param name="dto">The parameter</param>
        /// <param name="action">The action that should be called on the service</param>
        /// <param name="options">Optional options for this call</param>
        /// <returns>Return the result of this action</returns>
        Task<SuccessOrError<TResult>> InvokeService<TService, T, TResult>(T dto,
            Func<TService, Func<IServiceMessage<T>, ValueTask<SuccessOrError<TResult>>>> action,
            MethodOptions options = default) where TService : IConferenceService;

        /// <summary>
        ///     Invoke a method without a parameter but with a response value
        /// </summary>
        /// <typeparam name="TService">The type of the service</typeparam>
        /// <typeparam name="TResult">The type of the response value</typeparam>
        /// <param name="action">The action that should be called on the service</param>
        /// <param name="options">Optional options for this call</param>
        /// <returns>Return the result of this action</returns>
        Task<SuccessOrError<TResult>> InvokeService<TService, TResult>(
            Func<TService, Func<IServiceMessage, ValueTask<SuccessOrError<TResult>>>> action,
            MethodOptions options = default) where TService : IConferenceService;
    }
}
