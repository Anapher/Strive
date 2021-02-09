using System;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Interfaces
{
    /// <summary>
    ///     Indicate the result of an action that may have failed or returns a value
    /// </summary>
    /// <typeparam name="T">The type of the return value if the action did not fail</typeparam>
    public class SuccessOrError<T> : ISuccessOrError
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="SuccessOrError{T}" /> as succeeded with a return value
        /// </summary>
        /// <param name="response">The return value</param>
        public SuccessOrError(T response)
        {
            Response = response;
        }

        /// <summary>
        ///     Initialize a new instance of <see cref="SuccessOrError{T}" /> as failed
        /// </summary>
        /// <param name="error">The error that is responsible for the failure</param>
        public SuccessOrError(Error error)
        {
            Error = error;
        }

        /// <summary>
        ///     Initialize a new instance of <see cref="SuccessOrError{T}" />. Please not that either <see cref="Error" /> or
        ///     <see cref="Response" /> must be set
        /// </summary>
        public SuccessOrError()
        {
        }

#pragma warning disable CS8775 // Member must have a non-null value when exiting in some condition.
        [MemberNotNullWhen(false, nameof(Error))]
        [MemberNotNullWhen(true, nameof(Response))]
        public bool Success => Error == null;
#pragma warning restore CS8775 // Member must have a non-null value when exiting in some condition.

        public Error? Error { get; init; }

        /// <summary>
        ///     The return value of the action if succeeded
        /// </summary>
        public T? Response { get; init; }

        /// <summary>
        ///     Determine whether <see cref="Response" /> should be serialized
        /// </summary>
        /// <returns>Return true it should be serialized</returns>
        public bool ShouldSerializeResponse()
        {
            return Response != null && Response.GetType() != typeof(Unit);
        }

        /// <summary>
        ///     Create a new instance of <see cref="SuccessOrError{T}" /> as succeeded with a return value
        /// </summary>
        /// <param name="response">The return value</param>
        public static SuccessOrError<T> Succeeded(T response)
        {
            return new() {Response = response};
        }

        /// <summary>
        ///     Initialize a new instance of <see cref="SuccessOrError{T}" /> as failed
        /// </summary>
        /// <param name="error">The error that is responsible for the failure</param>
        public static SuccessOrError<T> Failed(Error error)
        {
            return new() {Error = error};
        }

        /// <summary>
        ///     Implicitly create a new <see cref="SuccessOrError{T}" /> that failed from an error
        /// </summary>
        /// <param name="error">The error that is responsible for the failure</param>
        public static implicit operator SuccessOrError<T>(Error error)
        {
            return Failed(error);
        }

        /// <summary>
        ///     Implicitly create a new instance of <see cref="SuccessOrError{T}" /> if the action did not fail with a return value
        /// </summary>
        /// <param name="response">The return value</param>
        public static implicit operator SuccessOrError<T>(T response)
        {
            if (response is Error)
                throw new InvalidOperationException("Cannot implicitly convert an error to a success result");

            return Succeeded(response);
        }
    }
}
