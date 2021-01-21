namespace PaderConference.Core.Services.Media.Communication
{
    public class ChangeStreamDto
    {
        public string? Id { get; set; }

        public Stream Type { get; set; }

        public StreamAction Action { get; set; }
    }

    public enum Stream
    {
        Producer,
        Consumer,
    }

    public enum StreamAction
    {
        Pause,
        Resume,
        Close,
    }
}
