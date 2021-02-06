using System.Collections.Generic;
using PaderConference.Core.Extensions;
using Xunit;

namespace PaderConference.Core.Tests.Extensions
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void TestEqualItemsEmptyDictionaries()
        {
            // arrange
            var dict1 = new Dictionary<string, int>();
            var dict2 = new Dictionary<string, int>();

            // act
            var result = dict1.EqualItems(dict2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void TestEqualItemsEqualDictionaries()
        {
            // arrange
            var dict1 = new Dictionary<string, int> {{"test", 5}, {"hello", 4}};
            var dict2 = new Dictionary<string, int> {{"hello", 4}, {"test", 5}};

            // act
            var result = dict1.EqualItems(dict2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void TestEqualItemsUnequalDictionaries()
        {
            // arrange
            var dict1 = new Dictionary<string, int> {{"test", 7}, {"hello", 4}};
            var dict2 = new Dictionary<string, int> {{"hello", 4}, {"test", 5}};

            // act
            var result = dict1.EqualItems(dict2);

            // assert
            Assert.False(result);
        }
    }
}
