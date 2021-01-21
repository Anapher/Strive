using System.Diagnostics.CodeAnalysis;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Interfaces
{
    /// <summary>
    ///     Indicate the result of an action that may have failed
    /// </summary>
    public class SuccessOrError : ISuccessOrError
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="SuccessOrError" /> that succeeded. You may prefer to use
        ///     <see cref="Succeeded" /> as it is more explicit
        /// </summary>
        public SuccessOrError()
        {
        }

        /// <summary>
        ///     Initialize a new instance of <see cref="SuccessOrError" /> as failed
        /// </summary>
        /// <param name="error">The error that is responsible for the failure</param>
        public SuccessOrError(Error error)
        {
            Error = error;
        }

        [MemberNotNullWhen(false, nameof(Error))]
        public bool Success => Error == null;

        public Error? Error { get; init; }

        /// <summary>
        ///     A constant that is simply a <see cref="SuccessOrError" /> instance that succeeded
        /// </summary>
        public static readonly SuccessOrError Succeeded = new();

        /// <summary>
        ///     Initialize a new instance of <see cref="SuccessOrError" /> as failed
        /// </summary>
        /// <param name="error">The error that is responsible for the failure</param>
        public static SuccessOrError Failed(Error error)
        {
            return new() {Error = error};
        }

        /// <summary>
        ///     Implicitly create a new <see cref="SuccessOrError" /> that failed from an error
        /// </summary>
        /// <param name="error">The error that is responsible for the failure</param>
        public static implicit operator SuccessOrError(Error error)
        {
            return Failed(error);
        }
    }
}
