using Duende.IdentityServer.Test;

namespace Identity.API.Quickstart
{
    public interface IUserProvider
    {
        bool ValidateCredentials(string username, string password);

        TestUser FindByUsername(string username);

        string IdToUsername(string id);
    }
}