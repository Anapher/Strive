using Strive.Core.Services.Poll.Types.Numeric;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.Numeric
{
    public class NumericAnswerValidatorTests
    {
        private readonly NumericAnswerValidator _validator = new();


        [Fact]
        public void Validate_SelectedIsBelowMin_ReturnError()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(5, null, null), new NumericAnswer(2));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_SelectedIsAboveMax_ReturnError()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(null, 5, null), new NumericAnswer(6));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_SelectedStepDoesNotMatch_ReturnError()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(null, null, 5), new NumericAnswer(12));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_SelectedDecimalStepDoesNotMatch_ReturnError()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(null, null, 0.5M), new NumericAnswer(0.75M));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_ValidDecimalStep_ReturnNull()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(null, null, 0.5M), new NumericAnswer(1.5M));

            // assert
            Assert.Null(result);
        }

        [Fact]
        public void Validate_ValidBetweenMinMax_ReturnNull()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(0, 1, 0.1M), new NumericAnswer(0.9M));

            // assert
            Assert.Null(result);
        }
    }
}
