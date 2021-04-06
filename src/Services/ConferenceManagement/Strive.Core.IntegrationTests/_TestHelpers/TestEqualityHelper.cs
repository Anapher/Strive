using Newtonsoft.Json.Linq;
using Xunit;

namespace Strive.Core.IntegrationTests._TestHelpers
{
    public class TestEqualityHelper
    {
        public static void AssertJsonConvertedEqual(object expected, object actual)
        {
            var expectedToken = JToken.FromObject(expected);
            var actualToken = JToken.FromObject(actual);

            Assert.True(JToken.DeepEquals(expectedToken, actualToken),
                $"JSON objects do not match. Expected:\r\n{expectedToken}\r\n\r\nActual:\r\n{actualToken}");
        }
    }
}
