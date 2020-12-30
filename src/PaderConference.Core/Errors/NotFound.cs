using System;

namespace PaderConference.Core.Errors
{
    /// <summary>
    ///     Indicates that the requested resource does not exist on the server.
    /// </summary>
    public record NotFoundError<TErrorCode> : DomainError<TErrorCode> where TErrorCode : Enum
    {
        public NotFoundError(string message, TErrorCode code) : base(ErrorType.NotFound, message, code)
        {
        }
    }
}
