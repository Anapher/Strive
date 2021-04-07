using System.Linq;
using Strive.Core.Extensions;
using Xunit;

namespace Strive.Core.Tests.Extensions
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
            Assert.Equal(test, Assert.Single(result));
        }
    }
}
