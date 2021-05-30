using System;
using Strive.Core.Services.Poll.Types.MultipleChoice;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.Tests.Services.Poll.Types.MultipleChoice
{
    public class MultipleChoiceAnswerValidatorTests
    {
        private readonly MultipleChoiceAnswerValidator _validator;

        public MultipleChoiceAnswerValidatorTests(ITestOutputHelper testOutputHelper)
        {
            var logger = testOutputHelper.CreateLogger<MultipleChoiceAnswerValidator>();
            _validator = new MultipleChoiceAnswerValidator(logger);
        }

        [Fact]
        public void Validate_SelectedIsEmpty_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null),
                new MultipleChoiceAnswer(Array.Empty<string>()));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_SelectedInvalidOption_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null),
                new MultipleChoiceAnswer(new[] {"C", "D"}));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_SelectedMoreThanPossible_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, 2),
                new MultipleChoiceAnswer(new[] {"A", "B", "C"}));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_SingleAnswer_ReturnTrue()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null),
                new MultipleChoiceAnswer(new[] {"A"}));

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Validate_ExactlyAsManyAsAllowed_ReturnTrue()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, 2),
                new MultipleChoiceAnswer(new[] {"A", "C"}));

            // assert
            Assert.True(result);
        }
    }
}
