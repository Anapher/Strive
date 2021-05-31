using Strive.Core.Services.Poll.Types.Numeric;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.Tests.Services.Poll.Types.Numeric
{
    public class NumericAnswerValidatorTests
    {
        private readonly NumericAnswerValidator _validator;

        public NumericAnswerValidatorTests(ITestOutputHelper testOutputHelper)
        {
            var logger = testOutputHelper.CreateLogger<NumericAnswerValidator>();
            _validator = new NumericAnswerValidator(logger);
        }

        [Fact]
        public void Validate_SelectedIsBelowMin_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(5, null, null), new NumericAnswer(2));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_SelectedIsAboveMax_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(null, 5, null), new NumericAnswer(6));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_SelectedStepDoesNotMatch_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(null, null, 5), new NumericAnswer(12));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_SelectedDecimalStepDoesNotMatch_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(null, null, 0.5M), new NumericAnswer(0.75M));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_ValidDecimalStep_ReturnTrue()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(null, null, 0.5M), new NumericAnswer(1.5M));

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Validate_ValidBetweenMinMax_ReturnTrue()
        {
            // act
            var result = _validator.Validate(new NumericInstruction(0, 1, 0.1M), new NumericAnswer(0.9M));

            // assert
            Assert.True(result);
        }
    }
}
