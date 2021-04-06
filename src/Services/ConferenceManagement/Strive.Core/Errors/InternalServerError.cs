using System;

namespace Strive.Core.Errors
{
    /// <summary>
    ///     Indicates that a generic error has occurred on the server.
    /// </summary>
    public record InternalServerError<TErrorCode> : DomainError<TErrorCode> where TErrorCode : Enum
    {
        public InternalServerError(string message, TErrorCode code) : base(ErrorType.InternalServerError, message, code)
        {
        }
    }
}
