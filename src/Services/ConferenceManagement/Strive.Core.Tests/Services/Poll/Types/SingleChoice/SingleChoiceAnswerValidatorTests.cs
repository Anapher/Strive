using Strive.Core.Services.Poll.Types.SingleChoice;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.SingleChoice
{
    public class SingleChoiceAnswerValidatorTests
    {
        private readonly SingleChoiceAnswerValidator _validator = new();

        [Fact]
        public void Validate_SelectedInvalidOption_ReturnError()
        {
            // act
            var result = _validator.Validate(new SingleChoiceInstruction(new[] {"A", "B", "C"}),
                new SingleChoiceAnswer("F"));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_SelectedMoreThanPossible_ReturnNull()
        {
            // act
            var result = _validator.Validate(new SingleChoiceInstruction(new[] {"A", "B", "C"}),
                new SingleChoiceAnswer("A"));

            // assert
            Assert.Null(result);
        }
    }
}
