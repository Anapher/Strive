namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public readonly struct TagCloudTag
    {
        public TagCloudTag(string submittedBy, string tag)
        {
            SubmittedBy = submittedBy;
            Tag = tag;
        }

        public string SubmittedBy { get; }
        public string Tag { get; }
    }
}
