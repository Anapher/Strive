using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public class TagCloudAggregator : IPollAnswerAggregator<TagCloudInstruction, TagCloudAnswer>
    {
        public ValueTask<PollResults> Aggregate(TagCloudInstruction instruction,
            IReadOnlyDictionary<string, TagCloudAnswer> answers)
        {
            var submittedTags = answers.SelectMany(x => x.Value.Tags.Select(tag => new TagCloudTag(x.Key, tag)))
                .ToList();

            var groups = TagCloudGrouper.GroupAnswers(instruction.Mode, submittedTags);

            return new ValueTask<PollResults>(new TagCloudPollResults(groups.ToDictionary(
                tagGroup => GetGroupTagName(tagGroup.Select(x => x.Tag)),
                tagGroup => (IReadOnlyList<string>) tagGroup.Select(x => x.SubmittedBy).ToList())));
        }

        private static string GetGroupTagName(IEnumerable<string> tags)
        {
            return tags.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        }
    }
}
