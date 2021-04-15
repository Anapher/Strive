using System;
using Duende.IdentityServer.Test;
using Identity.API.Quickstart;

namespace Identity.API
{
    public class DemoUserProvider : IUserProvider
    {
        public bool ValidateCredentials(string username, string password)
        {
            return !string.IsNullOrWhiteSpace(username);
        }

        public TestUser FindByUsername(string username)
        {
            return new()
            {
                Username = username,
                IsActive = true,
                SubjectId = Guid.NewGuid().ToString("N"),
                Password = "password",
            };
        }
    }
}