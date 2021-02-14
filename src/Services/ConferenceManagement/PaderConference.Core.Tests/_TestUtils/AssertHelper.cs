using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PaderConference.Core.Tests._TestUtils
{
    public static class AssertHelper
    {
        public static void AssertScrambledEquals<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.Equal(expected.OrderBy(x => x), actual.OrderBy(x => x));
        }
    }
}
