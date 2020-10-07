namespace PaderConference.Infrastructure.Services.Synchronization
{
    public class SynchronizedObjectUpdatedDto
    {
        public SynchronizedObjectUpdatedDto(string name, string patch)
        {
            Name = name;
            Patch = patch;
        }

        public string Name { get; set; }

        public string Patch { get; set; }
    }
}