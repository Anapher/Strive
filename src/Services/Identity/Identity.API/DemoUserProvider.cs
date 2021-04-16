using System;
using System.Text;
using System.Text.RegularExpressions;
using Duende.IdentityServer.Test;
using Identity.API.Quickstart;

namespace Identity.API
{
    public class DemoUserProvider : IUserProvider
    {
        public bool ValidateCredentials(string username, string password)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length < 12 &&
                   Regex.IsMatch(username, "^[a-zA-Z0-9]+$");
        }

        public TestUser FindByUsername(string username)
        {
            return new()
            {
                Username = username, IsActive = true, SubjectId = UsernameToUserId(username), Password = "password",
            };
        }

        public string UsernameToUserId(string username)
        {
            return Convert.ToHexString(Encoding.ASCII.GetBytes(username));
        }

        public string IdToUsername(string id)
        {
            return Encoding.ASCII.GetString(Convert.FromHexString(id));
        }
    }
}