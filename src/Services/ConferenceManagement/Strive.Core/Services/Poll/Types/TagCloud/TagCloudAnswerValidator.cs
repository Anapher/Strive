using System.Linq;
using Microsoft.Extensions.Logging;

namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public class TagCloudAnswerValidator : IPollAnswerValidator<TagCloudInstruction, TagCloudAnswer>
    {
        private readonly ILogger<TagCloudAnswerValidator> _logger;

        public TagCloudAnswerValidator(ILogger<TagCloudAnswerValidator> logger)
        {
            _logger = logger;
        }

        public bool Validate(TagCloudInstruction instruction, TagCloudAnswer answer)
        {
            if (!answer.Tags.Any())
            {
                _logger.LogDebug("Empty answers submitted");
                return false;
            }

            if (answer.Tags.Count > instruction.MaxTags)
            {
                _logger.LogDebug("Too many tags submitted (count: {count}, max: {max})", answer.Tags.Count,
                    instruction.MaxTags);
                return false;
            }

            if (TagCloudGrouper.GroupAnswers(instruction.Mode, answer.Tags.Select(x => new TagCloudTag("1", x)))
                .Any(x => x.Count > 1))
            {
                _logger.LogDebug("Duplicate tags submitted.");
                return false;
            }

            return true;
        }
    }
}
