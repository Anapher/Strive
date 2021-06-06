using Strive.Core.Services.Poll.Types.TagCloud;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.TagCloud
{
    public class TagCloudAnswerValidatorTests
    {
        private readonly TagCloudAnswerValidator _validator = new();


        [Fact]
        public void Validate_EmptyAnswers_ReturnError()
        {
            // arrange
            var answer = new TagCloudAnswer(new string[0]);
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.CaseInsensitive);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_TooManyAnswers_ReturnError()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"a", "b", "c", "d"});
            var instruction = new TagCloudInstruction(3, TagCloudClusterMode.CaseInsensitive);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_CaseInsensitive_DuplicateAnswers_ReturnError()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"visual basic", "Visual Basic"});
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.CaseInsensitive);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_Fuzzy_DuplicateAnswers_ReturnError()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"visual basic", "visualbasic"});
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.Fuzzy);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_Fuzzy_ValidAnswers_ReturnNull()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"visual basic", "c#"});
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.Fuzzy);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public void Validate_CaseInsensitive_ValidAnswers_ReturnNull()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"visual basic", "c#"});
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.CaseInsensitive);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.Null(result);
        }
    }
}
