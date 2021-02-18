using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PaderConference.Tests.Utils
{
    public static class AssertHelper
    {
        public static void AssertScrambledEquals<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.Equal(expected.OrderBy(x => x), actual.OrderBy(x => x));
        }
    }
}
