using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Strive.Core.Services.Poll.Types;
using Strive.Core.Services.Poll.Types.MultipleChoice;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.MultipleChoice
{
    public class MultipleChoiceAggregatorTests
    {
        [Fact]
        public async Task Aggregate_NoAnswers_ReturnEmpty()
        {
            // arrange
            var aggregator = new MultipleChoiceAggregator();
            var instruction = new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null);

            // act
            var result =
                await aggregator.Aggregate(instruction, ImmutableDictionary<string, MultipleChoiceAnswer>.Empty);

            // assert
            var multipleChoiceResults = Assert.IsType<SelectionPollResults>(result);
            Assert.Equal(multipleChoiceResults.Options.Keys, instruction.Options);
            Assert.All(multipleChoiceResults.Options, x => Assert.Empty(x.Value));
        }

        [Fact]
        public async Task Aggregate_SomeAnswers_ReturnResult()
        {
            // arrange
            var aggregator = new MultipleChoiceAggregator();
            var instruction = new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null);

            // act
            var result = await aggregator.Aggregate(instruction, new Dictionary<string, MultipleChoiceAnswer>
            {
                {"1", new MultipleChoiceAnswer(new[] {"B", "C"})},
                {"2", new MultipleChoiceAnswer(new[] {"A"})},
                {"3", new MultipleChoiceAnswer(new[] {"C", "A"})},
            });

            // assert
            var multipleChoiceResults = Assert.IsType<SelectionPollResults>(result);
            Assert.Equal(multipleChoiceResults.Options["A"], new[] {"2", "3"});
            Assert.Equal(multipleChoiceResults.Options["B"], new[] {"1"});
            Assert.Equal(multipleChoiceResults.Options["C"], new[] {"1", "3"});
        }
    }
}
