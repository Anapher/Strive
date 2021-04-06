using System;
using Strive.Core.Dto;

namespace Strive.Core.Errors
{
    public record DomainError<TCodeEnum> : Error where TCodeEnum : Enum
    {
        public DomainError(ErrorType errorType, string message, TCodeEnum code) : base(errorType.ToString(), message,
            code.ToString())
        {
        }
    }
}
