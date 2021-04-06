namespace Strive.Config
{
    public class AuthOptions
    {
        public string? Authority { get; set; }

        public string? Issuer { get; set; }

        public bool NoSslRequired { get; set; } = false;
    }
}
