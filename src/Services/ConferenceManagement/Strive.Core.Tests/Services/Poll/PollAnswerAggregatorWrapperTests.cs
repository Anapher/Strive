using System;
using System.Threading.Tasks;
using Autofac;
using Strive.Core.Services.Poll;
using Strive.Core.Services.Poll.Types.MultipleChoice;
using Strive.Core.Services.Poll.Types.SingleChoice;
using Strive.Core.Services.Poll.Utilities;
using Xunit;

namespace Strive.Core.Tests.Services.Poll
{
    public class PollAnswerAggregatorWrapperTests
    {
        private IComponentContext CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MultipleChoiceAggregator>().AsImplementedInterfaces();

            return builder.Build();
        }

        [Fact]
        public async Task AggregateAnswers_ValidInstruction_ReturnResult()
        {
            // arrange
            var container = CreateContainer();

            var wrapper = new PollAnswerAggregatorWrapper(container);
            var instruction = new MultipleChoiceInstruction(new[] {"A", "B"}, null);
            var answers = new[] {new PollAnswerWithKey(new MultipleChoiceAnswer(new[] {"A"}), "1")};

            // act
            var result = await wrapper.AggregateAnswers(instruction, answers);

            // assert
            Assert.IsType<MultipleChoicePollResults>(result);
        }

        [Fact]
        public async Task AggregateAnswers_InvalidAnswer_Throw()
        {
            // arrange
            var container = CreateContainer();

            var wrapper = new PollAnswerAggregatorWrapper(container);

            var instruction = new MultipleChoiceInstruction(new[] {"A", "B"}, null);
            var answers = new[] {new PollAnswerWithKey(new SingleChoiceAnswer("A"), "1")};

            // act
            await Assert.ThrowsAnyAsync<Exception>(async () => await wrapper.AggregateAnswers(instruction, answers));
        }
    }
}
