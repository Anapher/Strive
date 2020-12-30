using System;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Errors
{
    public record DomainError<TCodeEnum> : Error where TCodeEnum : Enum
    {
        public DomainError(ErrorType errorType, string message, TCodeEnum code) : base(errorType.ToString(), message,
            code.ToString())
        {
        }
    }
}
