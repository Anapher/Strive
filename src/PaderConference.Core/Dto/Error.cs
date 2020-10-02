using System.Collections.Generic;

namespace PaderConference.Core.Dto
{
    public class Error
    {
        public Error(string type, string message, int code, IReadOnlyDictionary<string, string>? fields = null)
        {
            Type = type;
            Message = message;
            Code = code;
            Fields = fields;
        }

        public string Type { get; }
        public string Message { get; }
        public int Code { get; }
        public IReadOnlyDictionary<string, string>? Fields { get; set; }
    }
}
