using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Moq;
using Strive.Core.Services.Poll.Types.MultipleChoice;
using Strive.Core.Services.Poll.Types.SingleChoice;
using Strive.Core.Services.Poll.Utilities;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Utilities
{
    public class PollAnswerValidatorWrapperTests
    {
        private IComponentContext CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MultipleChoiceAnswerValidator>().AsImplementedInterfaces();
            builder.RegisterInstance(new Mock<ILogger<MultipleChoiceAnswerValidator>>().Object)
                .AsImplementedInterfaces();

            return builder.Build();
        }

        [Fact]
        public void Validate_ValidInstruction_ReturnTrue()
        {
            // arrange
            var container = CreateContainer();

            var wrapper = new PollAnswerValidatorWrapper(container);

            var instruction = new MultipleChoiceInstruction(new[] {"A", "B"}, null);
            var answer = new MultipleChoiceAnswer(new[] {"A"});

            // act
            var result = wrapper.Validate(instruction, answer);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Validate_InvalidAnswer_ReturnFalse()
        {
            // arrange
            var container = CreateContainer();

            var wrapper = new PollAnswerValidatorWrapper(container);

            var instruction = new MultipleChoiceInstruction(new[] {"A", "B"}, null);
            var answer = new MultipleChoiceAnswer(new[] {"C"});

            // act
            var result = wrapper.Validate(instruction, answer);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_InvalidAnswerType_Throw()
        {
            // arrange
            var container = CreateContainer();

            var wrapper = new PollAnswerValidatorWrapper(container);

            var instruction = new MultipleChoiceInstruction(new[] {"A", "B"}, null);
            var answer = new SingleChoiceAnswer("A");

            // act
            Assert.ThrowsAny<Exception>(() => wrapper.Validate(instruction, answer));
        }
    }
}
