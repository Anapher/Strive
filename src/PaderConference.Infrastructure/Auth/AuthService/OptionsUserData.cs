#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace PaderConference.Infrastructure.Auth.AuthService
{
    public class OptionsUserData
    {
        public string Password { get; set; }

        public string Id { get; set; }

        public string DisplayName { get; set; }
    }
}
