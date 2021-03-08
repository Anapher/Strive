using System.Linq;
using JsonPatchGenerator.Tests._Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JsonPatchGenerator.Tests
{
    public class JsonPatchFactoryTests
    {
        public static TheoryData<string, string, string, string> TestData()
        {
            var testsString = EmbeddedResourceUtils.LoadResourceFile(typeof(JsonPatchFactoryTests).Assembly,
                "JsonPatchGenerator.Tests.Resources.tests.json");

            var tests = (JArray) JToken.Parse(testsString);
            var result = new TheoryData<string, string, string, string>();

            foreach (var test in tests.Cast<JObject>())
            {
                if (test.Property("error") != null) continue;
                if (test.Property("disabled") != null) continue;

                var doc = test.Property("doc")!.Value;
                var expected = test.Property("expected")!.Value;
                var patch = test.Property("patch")!.Value;
                var comment = test.Property("comment")?.Value.Value<string>() ?? string.Empty;

                result.Add(doc.ToString(Formatting.None), expected.ToString(Formatting.None),
                    patch.ToString(Formatting.None), comment);
            }

            return result;
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestPatch(string original, string modified, string expectedPatch, string comment)
        {
            var originalToken = JToken.Parse(original);
            var modifiedToken = JToken.Parse(modified);

            // generate patch
            var actualPatch = JsonPatchFactory.Create(originalToken, modifiedToken, JsonPatchFactory.DefaultOptions);

            // assert
            var originalObj = originalToken.ToObject<dynamic>();
            actualPatch.ApplyTo(originalObj);

            var resultingToken = JToken.FromObject(originalObj);

            Assert.True(JToken.DeepEquals(resultingToken, modifiedToken),
                $"Comment: {comment}\r\nexpected patch: {expectedPatch}\r\nactual patch: {JToken.FromObject(actualPatch)}");
        }
    }
}