using PaderConference.Infrastructure.Extensions;
using Xunit;

namespace PaderConference.Infrastructure.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("dGVzdA==", "dGVzdA")]
        [InlineData("5Cv89ivk9iv8I/YrKyMrIw==", "5Cv89ivk9iv8I_YrKyMrIw")]
        [InlineData("", "")]
        public void ToUrlBase64_TestValues_IsCorrect(string base64, string expectedUrlBase64)
        {
            var actualUrlBase64 = base64.ToUrlBase64();
            Assert.Equal(expectedUrlBase64, actualUrlBase64);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("asd", "asd")]
        [InlineData("HelloWorld", "helloWorld")]
        [InlineData("?Why", "?Why")]
        public void ToCamelCase_TestValues_IsCorrect(string original, string expected)
        {
            var actual = original.ToCamelCase();
            Assert.Equal(expected, actual);
        }
    }
}
