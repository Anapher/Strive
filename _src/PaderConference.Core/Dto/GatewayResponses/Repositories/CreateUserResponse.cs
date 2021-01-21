namespace PaderConference.Core.Dto.GatewayResponses.Repositories
{
    public sealed class CreateUserResponse : BaseGatewayResponse
    {
        public string Id { get; }

        public CreateUserResponse(string id, bool success = false, Error? error = null) : base(success, error)
        {
            Id = id;
        }
    }
}
