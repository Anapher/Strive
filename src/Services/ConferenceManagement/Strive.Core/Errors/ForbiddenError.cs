using System;

namespace Strive.Core.Errors
{
    /// <summary>
    ///     Indicates that the server refuses to fulfill the request.
    /// </summary>
    public record ForbiddenError<TErrorCode> : DomainError<TErrorCode> where TErrorCode : Enum
    {
        public ForbiddenError(string message, TErrorCode code) : base(ErrorType.Forbidden, message, code)
        {
        }
    }
}
