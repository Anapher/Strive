namespace Strive.Core.Services.Media
{
    public class SfuConnectionOptions
    {
        public SfuConnectionOptions(string urlTemplate)
        {
            UrlTemplate = urlTemplate;
        }

        public string UrlTemplate { get; set; }
    }
}
