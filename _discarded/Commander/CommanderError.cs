using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Infrastructure.Commander
{
    public class CommanderError
    {
        public static Error ServiceNotFound =>
            new FieldValidationError(nameof(ServiceCommandDto.Service), "The service was not found.");

        public static Error MethodNotFound =>
            new FieldValidationError(nameof(ServiceCommandDto.Method), "The method was not found.");

        public static Error PayloadMustNotBeNull =>
            new FieldValidationError(nameof(ServiceCommandDto.Payload), "The payload must not be null.");
    }
}
