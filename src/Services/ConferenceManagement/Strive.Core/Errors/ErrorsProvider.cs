using System;
using Strive.Core.Dto;

namespace Strive.Core.Errors
{
    public class ErrorsProvider<TErrorCode> where TErrorCode : Enum
    {
        protected static Error BadRequest(string message, TErrorCode code)
        {
            return new BadRequestError<TErrorCode>(message, code);
        }

        protected static Error Conflict(string message, TErrorCode code)
        {
            return new ConflictError<TErrorCode>(message, code);
        }

        protected static Error InternalServerError(string message, TErrorCode code)
        {
            return new InternalServerError<TErrorCode>(message, code);
        }

        protected static Error Forbidden(string message, TErrorCode code)
        {
            return new ForbiddenError<TErrorCode>(message, code);
        }

        protected static Error NotFound(string message, TErrorCode code)
        {
            return new NotFoundError<TErrorCode>(message, code);
        }
    }
}
