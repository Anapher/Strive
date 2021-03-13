using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PaderConference.Core.Extensions;
using Xunit;

namespace PaderConference.Core.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        public static readonly TheoryData<IReadOnlyList<string>, IReadOnlyList<string>> EqualLists = new()
        {
            {ImmutableList<string>.Empty, ImmutableList<string>.Empty},
            {new[] {"1", "2", "3"}, new[] {"1", "2", "3"}},
            {new[] {"1", "2", "3"}, new[] {"3", "2", "1"}},
            {new[] {"1", "2", "1", "3"}, new[] {"3", "1", "2", "1"}},
        };

        public static readonly TheoryData<IReadOnlyList<string>, IReadOnlyList<string>> UnequalLists = new()
        {
            {ImmutableList<string>.Empty, new[] {"1"}},
            {new[] {"1", "2", "3"}, new[] {"1", "2"}},
            {new[] {"1", "3"}, new[] {"3", "2", "1"}},
            {new[] {"1", "2", "1", "3"}, new[] {"3", "1", "2"}},
        };


        [Theory]
        [MemberData(nameof(EqualLists))]
        public void ScrambledEquals_EqualLists_ReturnTrue(IReadOnlyList<string> list1, IReadOnlyList<string> list2)
        {
            var result = list1.ScrambledEquals(list2);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(UnequalLists))]
        public void ScrambledEquals_UnequalLists_ReturnFalse(IReadOnlyList<string> list1, IReadOnlyList<string> list2)
        {
            var result = list1.ScrambledEquals(list2);
            Assert.False(result);
        }

        [Fact]
        public void WhereNotNull_EmptyEnumerable_ReturnEmpty()
        {
            var result = Enumerable.Empty<string>().WhereNotNull();
            Assert.Empty(result);
        }

        [Fact]
        public void WhereNotNull_HasNullValues_ReturnOnlyNonNull()
        {
            var result = new[] {"1", null, "5", "6", null}.WhereNotNull();
            Assert.Equal(new[] {"1", "5", "6"}, result);
        }
    }
}
