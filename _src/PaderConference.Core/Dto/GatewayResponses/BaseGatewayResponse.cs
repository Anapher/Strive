namespace PaderConference.Core.Dto.GatewayResponses
{
    public abstract class BaseGatewayResponse
    {
        public bool Success { get; }
        public Error? Error { get; }

        protected BaseGatewayResponse(bool success = false, Error? error = null)
        {
            Success = success;
            Error = error;
        }
    }
}
