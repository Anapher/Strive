using System;

namespace Strive.Core.Errors
{
    /// <summary>
    ///     Indicates that the request could not be carried out because of a conflict on the server.
    /// </summary>
    public record ConflictError<TErrorCode> : DomainError<TErrorCode> where TErrorCode : Enum
    {
        public ConflictError(string message, TErrorCode code) : base(ErrorType.Conflict, message, code)
        {
        }
    }
}
