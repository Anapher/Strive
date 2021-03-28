namespace PaderConference.Config
{
    public class AuthOptions
    {
        public string? Authority { get; set; }

        public bool NoSslRequired { get; set; } = false;
    }
}
