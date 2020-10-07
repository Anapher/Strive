namespace PaderConference.Infrastructure.Services.Media
{
    public class SynchronizedMedia
    {
        public SynchronizedMedia(bool isScreenshareActivated = false, string? partipantScreensharing = null)
        {
            IsScreenshareActivated = isScreenshareActivated;
            PartipantScreensharing = partipantScreensharing;
        }

        public bool IsScreenshareActivated { get; }
        public string? PartipantScreensharing { get; }
    }
}