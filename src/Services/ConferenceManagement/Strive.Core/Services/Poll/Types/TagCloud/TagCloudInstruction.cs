namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public record TagCloudInstruction(int? MaxTags, TagCloudClusterMode Mode) : PollInstruction<TagCloudAnswer>;
}
