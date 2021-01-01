using System.Collections.Generic;

namespace PaderConference.Infrastructure.Auth.AuthService
{
    public class UserCredentialsOptions
    {
        public Dictionary<string, OptionsUserData>? Users { get; set; }
    }
}
