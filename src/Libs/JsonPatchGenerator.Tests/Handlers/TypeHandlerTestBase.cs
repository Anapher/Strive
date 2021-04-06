using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JsonPatchGenerator.Tests.Handlers
{
    public abstract class TypeHandlerTestBase
    {
        protected readonly IPatchContext Context;
        protected readonly JsonPatchDocument Document;

        protected readonly List<(JToken original, JToken modified, JsonPatchPath path)> PatchCalls =
            new();

        protected TypeHandlerTestBase()
        {
            Document = new JsonPatchDocument();
            Context = CreatePatchContext();
        }

        protected IPatchContext CreatePatchContext()
        {
            var mock = new Mock<IPatchContext>();

            mock.SetupGet(x => x.Document).Returns(Document);
            mock.SetupGet(x => x.Options).Returns(new JsonPatchOptions());
            mock.Setup(x => x.CreatePatch(It.IsAny<JToken>(), It.IsAny<JToken>(), It.IsAny<JsonPatchPath>()))
                .Callback((JToken original, JToken modified, JsonPatchPath path) =>
                {
                    PatchCalls.Add((original, modified, path));
                });

            return mock.Object;
        }

        protected void AssertJTokenEqualsValue(object? expected, JToken token)
        {
            var valueToken = expected == null ? JValue.CreateNull() : JToken.FromObject(expected);
            Assert.True(JToken.DeepEquals(valueToken, token),
                $"The values does not match. expected: {valueToken}, actual: {token}");
        }

        protected void AssertSinglePatchCall(string expectedPath, object? expectedOriginal, object? expectedModified)
        {
            var (originalValue, modifiedValue, path) = Assert.Single(PatchCalls);
            Assert.Equal(expectedPath, path.ToString());
            AssertJTokenEqualsValue(expectedOriginal, originalValue);
            AssertJTokenEqualsValue(expectedModified, modifiedValue);
        }
    }
}