using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JsonPatchGenerator.Tests._Utils
{
    public static class AssertHelper
    {
        public static void AssertScrambledEquals<T>(IEnumerable<T> expected, IEnumerable<T> actual,
            IEqualityComparer<T> comparer)
        {
            if (typeof(T).IsAssignableTo(typeof(IComparable)))
            {
                Assert.Equal(expected.OrderBy(x => x), actual.OrderBy(x => x));
            }
            else
            {
                var expectedList = expected.ToList();
                var actualList = actual.ToList();
                Assert.Equal(expectedList.Count, actualList.Count);

                foreach (var expectedItem in expectedList)
                {
                    Assert.Contains(expectedItem, actualList, comparer);
                }
            }
        }
    }
}