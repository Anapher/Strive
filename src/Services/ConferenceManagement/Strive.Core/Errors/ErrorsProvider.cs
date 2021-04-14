using System;
using System.Collections.Generic;
using Strive.Core.Dto;

namespace Strive.Core.Errors
{
    public class ErrorsProvider<TErrorCode> where TErrorCode : Enum
    {
        protected static Error BadRequest(string message, TErrorCode code,
            IReadOnlyDictionary<string, string>? fields = null)
        {
            return new BadRequestError<TErrorCode>(message, code) {Fields = fields};
        }

        protected static Error Conflict(string message, TErrorCode code,
            IReadOnlyDictionary<string, string>? fields = null)
        {
            return new ConflictError<TErrorCode>(message, code) {Fields = fields};
        }

        protected static Error InternalServerError(string message, TErrorCode code,
            IReadOnlyDictionary<string, string>? fields = null)
        {
            return new InternalServerError<TErrorCode>(message, code) {Fields = fields};
        }

        protected static Error Forbidden(string message, TErrorCode code,
            IReadOnlyDictionary<string, string>? fields = null)
        {
            return new ForbiddenError<TErrorCode>(message, code) {Fields = fields};
        }

        protected static Error NotFound(string message, TErrorCode code,
            IReadOnlyDictionary<string, string>? fields = null)
        {
            return new NotFoundError<TErrorCode>(message, code) {Fields = fields};
        }
    }
}
