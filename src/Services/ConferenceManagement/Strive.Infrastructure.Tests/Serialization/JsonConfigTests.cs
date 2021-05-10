using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Strive.Infrastructure.Serialization;
using Xunit;

namespace Strive.Infrastructure.Tests.Serialization
{
    public class JsonConfigTests
    {
        [Fact]
        public void Serialize_DictionaryWithStringKeys_KeepCasing()
        {
            // arrange
            var dict = new Dictionary<string, int> {{"Olaf", 1}, {"vincent", 2}};

            // act
            var token = (JObject) Serialize(dict);

            // assert
            Assert.NotNull(token.Property("Olaf"));
            Assert.NotNull(token.Property("vincent"));
        }

        [Fact]
        public void Serialize_DictionaryWithEnumKeys_CamelCase()
        {
            // arrange
            var dict = new Dictionary<TestEnum, int> {{TestEnum.TestProp1, 1}, {TestEnum.testProp2, 2}};

            // act
            var token = (JObject) Serialize(dict);

            // assert
            Assert.NotNull(token.Property("testProp1"));
            Assert.NotNull(token.Property("testProp2"));
        }

        private static JToken Serialize(object obj)
        {
            return JToken.FromObject(obj, JsonSerializer.Create(JsonConfig.Default));
        }

        public enum TestEnum
        {
            TestProp1,
            testProp2,
        }
    }
}
