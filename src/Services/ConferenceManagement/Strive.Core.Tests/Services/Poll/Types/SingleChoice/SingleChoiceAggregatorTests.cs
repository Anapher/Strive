using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Strive.Core.Services.Poll.Types;
using Strive.Core.Services.Poll.Types.SingleChoice;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.SingleChoice
{
    public class SingleChoiceAggregatorTests
    {
        [Fact]
        public async Task Aggregate_NoAnswers_ReturnEmpty()
        {
            // arrange
            var aggregator = new SingleChoiceAggregator();
            var instruction = new SingleChoiceInstruction(new[] {"A", "B", "C"});

            // act
            var result = await aggregator.Aggregate(instruction, ImmutableDictionary<string, SingleChoiceAnswer>.Empty);

            // assert
            var multipleChoiceResults = Assert.IsType<SelectionPollResults>(result);
            Assert.Equal(multipleChoiceResults.Options.Keys, instruction.Options);
            Assert.All(multipleChoiceResults.Options, x => Assert.Empty(x.Value));
        }

        [Fact]
        public async Task Aggregate_SomeAnswers_ReturnResult()
        {
            // arrange
            var aggregator = new SingleChoiceAggregator();
            var instruction = new SingleChoiceInstruction(new[] {"A", "B", "C"});

            // act
            var result = await aggregator.Aggregate(instruction, new Dictionary<string, SingleChoiceAnswer>
            {
                {"1", new SingleChoiceAnswer("A")},
                {"2", new SingleChoiceAnswer("A")},
                {"3", new SingleChoiceAnswer("B")},
            });

            // assert
            var multipleChoiceResults = Assert.IsType<SelectionPollResults>(result);
            Assert.Equal(multipleChoiceResults.Options["A"], new[] {"1", "2"});
            Assert.Equal(multipleChoiceResults.Options["B"], new[] {"3"});
            Assert.Empty(multipleChoiceResults.Options["C"]);
        }
    }
}
