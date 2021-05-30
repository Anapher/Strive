using Strive.Core.Services.Poll.Types.SingleChoice;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.Tests.Services.Poll.Types.SingleChoice
{
    public class SingleChoiceAnswerValidatorTests
    {
        private readonly SingleChoiceAnswerValidator _validator;

        public SingleChoiceAnswerValidatorTests(ITestOutputHelper testOutputHelper)
        {
            var logger = testOutputHelper.CreateLogger<SingleChoiceAnswerValidator>();
            _validator = new SingleChoiceAnswerValidator(logger);
        }

        [Fact]
        public void Validate_SelectedInvalidOption_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new SingleChoiceInstruction(new[] {"A", "B", "C"}),
                new SingleChoiceAnswer("F"));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_SelectedMoreThanPossible_ReturnFalse()
        {
            // act
            var result = _validator.Validate(new SingleChoiceInstruction(new[] {"A", "B", "C"}),
                new SingleChoiceAnswer("A"));

            // assert
            Assert.True(result);
        }
    }
}
