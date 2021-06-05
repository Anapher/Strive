using System;
using Strive.Core.Services.Poll.Types.MultipleChoice;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.MultipleChoice
{
    public class MultipleChoiceAnswerValidatorTests
    {
        private readonly MultipleChoiceAnswerValidator _validator = new();

        [Fact]
        public void Validate_SelectedIsEmpty_ReturnError()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null),
                new MultipleChoiceAnswer(Array.Empty<string>()));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_SelectedInvalidOption_ReturnError()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null),
                new MultipleChoiceAnswer(new[] {"C", "D"}));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_SelectedMoreThanPossible_ReturnError()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, 2),
                new MultipleChoiceAnswer(new[] {"A", "B", "C"}));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_SingleAnswer_ReturnNull()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null),
                new MultipleChoiceAnswer(new[] {"A"}));

            // assert
            Assert.Null(result);
        }

        [Fact]
        public void Validate_ExactlyAsManyAsAllowed_ReturnNull()
        {
            // act
            var result = _validator.Validate(new MultipleChoiceInstruction(new[] {"A", "B", "C"}, 2),
                new MultipleChoiceAnswer(new[] {"A", "C"}));

            // assert
            Assert.Null(result);
        }
    }
}
