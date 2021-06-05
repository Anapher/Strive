using System.Linq;
using Strive.Core.Dto;

namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public class TagCloudAnswerValidator : IPollAnswerValidator<TagCloudInstruction, TagCloudAnswer>
    {
        public Error? Validate(TagCloudInstruction instruction, TagCloudAnswer answer)
        {
            if (!answer.Tags.Any())
            {
                return PollError.AnswerValidationFailed("The answer does not contain any tags");
            }

            if (answer.Tags.Count > instruction.MaxTags)
            {
                return PollError.AnswerValidationFailed(
                    $"Too many tags submitted (count: {answer.Tags.Count}, max: {instruction.MaxTags})");
            }

            var groups =
                TagCloudGrouper.GroupAnswers(instruction.Mode, answer.Tags.Select(x => new TagCloudTag("1", x)));

            if (groups.Any(x => x.Count > 1))
            {
                var duplicateTags = groups.Where(x => x.Count > 1).ToList();
                return PollError.AnswerValidationFailed(
                    $"The answer does contain {duplicateTags.Sum(x => x.Count)} duplicate tags. The following tags are an equal group: {string.Join(", ", groups.Select(x => "(" + string.Join(", ", x.Select(t => t.Tag)) + ")"))}.");
            }

            return null;
        }
    }
}
