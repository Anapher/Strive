using Strive.Core.Services.Poll.Types.TagCloud.Clustering;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.TagCloud.Clustering
{
    public class NormalizedLevenshteinDistanceTests
    {
        private readonly NormalizedLevenshteinDistance _calculator = new();

        [Fact]
        public void LevenshteinDistance_EqualStrings_ReturnZero()
        {
            var s = "hello";

            var result = _calculator.CalculateDistance(s, s);

            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData("c#", "c #")]
        [InlineData("java", "jva")]
        [InlineData("Golden Retriver", "GoldenRetriver")]
        [InlineData("C++", "C+++")]
        public void LevenshteinDistance_Similar_ReturnHighSimilarity(string x, string y)
        {
            var result = _calculator.CalculateDistance(x, y);

            Assert.True(result < NormalizedLevenshteinDistance.Threshold);
        }

        [Theory]
        [InlineData("c#", "c++")]
        [InlineData("c#", "c+")]
        public void LevenshteinDistance_NotSimilar_ReturnLowSimilarity(string x, string y)
        {
            var result = _calculator.CalculateDistance(x, y);

            Assert.True(result > NormalizedLevenshteinDistance.Threshold);
        }
    }
}
