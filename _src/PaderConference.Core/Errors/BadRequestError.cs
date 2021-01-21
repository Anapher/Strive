using System;

namespace PaderConference.Core.Errors
{
    /// <summary>
    ///     Indicates that the request could not be understood by the server. Sent when
    ///     no other error is applicable, or if the exact error is unknown or does not have its own error code.
    /// </summary>
    public record BadRequestError<TErrorCode> : DomainError<TErrorCode> where TErrorCode : Enum
    {
        public BadRequestError(string message, TErrorCode code) : base(ErrorType.BadRequest, message, code)
        {
        }
    }
}
