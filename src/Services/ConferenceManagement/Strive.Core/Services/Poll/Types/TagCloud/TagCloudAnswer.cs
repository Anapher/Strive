using System.Collections.Generic;

namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public record TagCloudAnswer(IReadOnlyList<string> Tags) : PollAnswer;
}
