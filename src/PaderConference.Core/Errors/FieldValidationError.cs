using System;
using System.Collections.Generic;
using System.Linq;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Errors
{
    public record FieldValidationError : Error
    {
        public FieldValidationError(IReadOnlyDictionary<string, string> fieldErrors,
            ErrorCode code = ErrorCode.FieldValidation) : base(ErrorType.BadRequest.ToString(),
            "Request validation failed.", code.ToString())
        {
            if (!fieldErrors.Any()) throw new ArgumentException("You must give at least one field error.", nameof(fieldErrors));
            Fields = fieldErrors;
        }

        public FieldValidationError(string name, string error, ErrorCode code = ErrorCode.FieldValidation) : this(new Dictionary<string, string> { { name, error } }, code)
        {
        }
    }
}
