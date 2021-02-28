using PaderConference.Core.Utilities;
using Xunit;

namespace PaderConference.Core.Tests.Utilities
{
    public class IndexUtilsTests
    {
        [Theory]
        [InlineData(0, 50, 0, 0, -1)]
        [InlineData(0, 50, 1, 0, 0)]
        [InlineData(0, 100, 50, 0, 49)]
        [InlineData(0, -1, 50, 0, 49)]
        [InlineData(-10, -1, 50, 40, 49)]
        [InlineData(-1, 0, 50, 0, 49)]
        [InlineData(-1, 100, 50, 49, 49)]
        public void TestTranslateStartEndIndex(int start, int end, int total, int expectedStart, int expectedEnd)
        {
            var (actualStart, actualEnd) = IndexUtils.TranslateStartEndIndex(start, end, total);
            Assert.Equal(expectedStart, actualStart);
            Assert.Equal(expectedEnd, actualEnd);
        }
    }
}
