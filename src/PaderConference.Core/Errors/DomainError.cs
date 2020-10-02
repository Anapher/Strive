using PaderConference.Core.Dto;
using System.Collections.Generic;

namespace PaderConference.Core.Errors
{
    public class DomainError : Error
    {
        public DomainError(ErrorType errorType, string message, ErrorCode code, IReadOnlyDictionary<string, string>? fields = null) : base(errorType.ToString(), message, (int) code, fields)
        {
        }

        public DomainError SetField(string fieldName, string fieldError)
        {
            Fields = new Dictionary<string, string> { { fieldName, fieldError } };
            return this;
        }

        public DomainError SetField(string fieldName)
        {
            SetField(fieldName, Message);
            return this;
        }

        public DomainError SetFields(IReadOnlyDictionary<string, string> fields)
        {
            Fields = fields;
            return this;
        }
    }
}
