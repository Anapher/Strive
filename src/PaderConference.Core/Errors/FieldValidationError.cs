using PaderConference.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderConference.Core.Errors
{
    public class FieldValidationError : Error
    {
        public FieldValidationError(IReadOnlyDictionary<string, string> fieldErrors, ErrorCode code = ErrorCode.FieldValidation) : base(ErrorType.ValidationError.ToString(), "Request validation failed.", (int)code, fieldErrors)
        {
            if (!fieldErrors.Any()) throw new ArgumentException("You must give at least one field error.", nameof(fieldErrors));
        }

        public FieldValidationError(string name, string error, ErrorCode code = ErrorCode.FieldValidation) : this(new Dictionary<string, string> { { name, error } }, code)
        {
        }
    }
}
