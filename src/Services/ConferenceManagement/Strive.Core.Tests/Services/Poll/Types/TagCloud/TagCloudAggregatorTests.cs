using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Services.Poll.Types.TagCloud;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.TagCloud
{
    public class TagCloudAggregatorTests
    {
        [Fact]
        public async Task Aggregate_CaseInsensitive_BuildCloud()
        {
            // arrange
            var aggregator = new TagCloudAggregator();
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.CaseInsensitive);

            var answers = new Dictionary<string, TagCloudAnswer>
            {
                {"1", new TagCloudAnswer(new[] {"C#", "Visual Basic", "java"})},
                {"2", new TagCloudAnswer(new[] {"C#", "Go", "Java"})},
                {"3", new TagCloudAnswer(new[] {"Java", "Go", "C", "javaa"})},
            };

            // act
            var result = (TagCloudPollResults) await aggregator.Aggregate(instruction, answers);

            // assert
            AssertCluster(new Dictionary<string, IReadOnlyList<string>>
            {
                {
                    "C#", new[] {"1", "2"}
                },
                {
                    "Visual Basic", new[] {"1"}
                },
                {
                    "Java", new[] {"1", "2", "3"}
                },
                {
                    "Go", new[] {"2", "3"}
                },
                {
                    "C", new[] {"3"}
                },
                {
                    "javaa", new[] {"3"}
                },
            }, result.Tags);
        }

        [Fact]
        public async Task Aggregate_Fuzzy_BuildCloud()
        {
            // arrange
            var aggregator = new TagCloudAggregator();
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.Fuzzy);

            var answers = new Dictionary<string, TagCloudAnswer>
            {
                {"1", new TagCloudAnswer(new[] {"visual Basic", "java", "goo"})},
                {"2", new TagCloudAnswer(new[] {"Visual Basic", "go"})},
                {"3", new TagCloudAnswer(new[] {"Visual Basic", "go"})},
                {"4", new TagCloudAnswer(new[] {"visualbasic"})},
            };

            // act
            var result = (TagCloudPollResults) await aggregator.Aggregate(instruction, answers);

            // assert
            AssertCluster(new Dictionary<string, IReadOnlyList<string>>
            {
                {
                    "Visual Basic", new[] {"1", "2", "3", "4"}
                },
                {
                    "java", new[] {"1"}
                },
                {
                    "go", new[] {"1", "2", "3"}
                },
            }, result.Tags);
        }

        private static void AssertCluster(IReadOnlyDictionary<string, IReadOnlyList<string>> expected,
            IReadOnlyDictionary<string, IReadOnlyList<string>> actual)
        {
            AssertHelper.AssertDictionariesEqual(expected.ToDictionary(x => x.Key, x => x.Value.ToHashSet()),
                actual.ToDictionary(x => x.Key, x => x.Value.ToHashSet()), HashSet<string>.CreateSetComparer());
        }
    }
}
