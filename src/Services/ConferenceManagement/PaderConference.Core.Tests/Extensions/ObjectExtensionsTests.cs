using System.Linq;
using PaderConference.Core.Extensions;
using Xunit;

namespace PaderConference.Core.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void TestYield()
        {
            // arrange
            var test = "hello";

            // act
            var result = test.Yield().ToList();

            // assert
            var actual = result.Single();
            Assert.Equal(test, actual);
        }
    }
}
