using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services
{
    public class ServiceError : Error
    {
        public ServiceError(string message, ServiceErrorCode code) : base(ErrorType.ServiceError.ToString(), message,
            (int) code)
        {
        }
    }
}