using System.Diagnostics.CodeAnalysis;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Interfaces
{
    /// <summary>
    ///     Indicate the result of an action that may have failed.
    /// </summary>
    public interface ISuccessOrError
    {
        /// <summary>
        ///     Return true if the action succeeded
        /// </summary>
        [MemberNotNullWhen(false, nameof(Error))]
        bool Success { get; }

        /// <summary>
        ///     If <see cref="Success" /> returns false, return the error that is responsible.
        /// </summary>
        public Error? Error { get; }
    }
}
