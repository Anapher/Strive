using System.Collections.Generic;

namespace Strive.Core.Dto
{
    /// <summary>
    ///     Information about an error
    /// </summary>
    public record Error
    {
        public Error(string type, string message, string code)
        {
            Type = type;
            Message = message;
            Code = code;
        }

        /// <summary>
        ///     The type of the error
        /// </summary>
        public string Type { get; }

        /// <summary>
        ///     A short message describing the error
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     A unique code for the error
        /// </summary>
        public string Code { get; }

        /// <summary>
        ///     Fields that must be changed to fix this error
        /// </summary>
        public IReadOnlyDictionary<string, string>? Fields { get; init; }

        public override string ToString()
        {
            return $"An error occurred! {Type}\r\nCode: {Code}\r\nMessage: {Message}";
        }
    }
}
