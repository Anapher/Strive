using Strive.Core.Services.Poll.Types.TagCloud;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.Tests.Services.Poll.Types.TagCloud
{
    public class TagCloudAnswerValidatorTests
    {
        private readonly TagCloudAnswerValidator _validator;

        public TagCloudAnswerValidatorTests(ITestOutputHelper testOutputHelper)
        {
            var logger = testOutputHelper.CreateLogger<TagCloudAnswerValidator>();
            _validator = new TagCloudAnswerValidator(logger);
        }

        [Fact]
        public void Validate_EmptyAnswers_ReturnFalse()
        {
            // arrange
            var answer = new TagCloudAnswer(new string[0]);
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.CaseInsensitive);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_TooManyAnswers_ReturnFalse()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"a", "b", "c", "d"});
            var instruction = new TagCloudInstruction(3, TagCloudClusterMode.CaseInsensitive);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_CaseInsensitive_DuplicateAnswers_ReturnFalse()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"visual basic", "Visual Basic"});
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.CaseInsensitive);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_Fuzzy_DuplicateAnswers_ReturnFalse()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"visual basic", "visualbasic"});
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.Fuzzy);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_Fuzzy_ValidAnswers_ReturnTrue()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"visual basic", "c#"});
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.Fuzzy);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Validate_CaseInsensitive_ValidAnswers_ReturnTrue()
        {
            // arrange
            var answer = new TagCloudAnswer(new[] {"visual basic", "c#"});
            var instruction = new TagCloudInstruction(null, TagCloudClusterMode.CaseInsensitive);

            // act
            var result = _validator.Validate(instruction, answer);

            // assert
            Assert.True(result);
        }
    }
}
