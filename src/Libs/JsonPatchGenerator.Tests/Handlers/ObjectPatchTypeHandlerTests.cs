using JsonPatchGenerator.Handlers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JsonPatchGenerator.Tests.Handlers
{
    public class ObjectPatchTypeHandlerTests : TypeHandlerTestBase
    {
        private readonly ObjectPatchTypeHandler _handler = new();

        [Fact]
        public void CanPatch_SupplyObject_ReturnTrue()
        {
            // arrange
            var original = JToken.FromObject(new TestObj {Value = 5});
            var modified = JToken.FromObject(new TestObj {Value = 435});

            // act
            var canPatch = _handler.CanPatch(original, modified);

            // assert
            Assert.True(canPatch);
        }

        [Fact]
        public void CreatePatch_ValueChanged_CallPatchValue()
        {
            // arrange
            var original = JToken.FromObject(new TestObj {Value = 5});
            var modified = JToken.FromObject(new TestObj {Value = 435});

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertSinglePatchCall("/Value", 5, 435);
        }

        [Fact]
        public void CreatePatch_PropertyDidNotExist_CallPatchValue()
        {
            // arrange
            var original = JToken.Parse("{}");
            var modified = JToken.FromObject(new TestObj2 {Value = "hello"});

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertSinglePatchCall("/Value", null, "hello");
        }

        [Fact]
        public void CreatePatch_PropertyWasNull_CallPatchValue()
        {
            // arrange
            var original = JToken.FromObject(new TestObj2 {Value = null});
            var modified = JToken.FromObject(new TestObj2 {Value = "hello"});

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertSinglePatchCall("/Value", null, "hello");
        }

        [Fact]
        public void CreatePatch_PropertyDoesNotExistAnymore_CallPatchValueSetNull()
        {
            // arrange
            var original = JToken.FromObject(new TestObj2 {Value = "hello"});
            var modified = JToken.Parse("{}");

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertSinglePatchCall("/Value", "hello", null);
        }

        [Fact]
        public void CreatePatch_PropertyNull_CallPatchValueSetNull()
        {
            // arrange
            var original = JToken.FromObject(new TestObj2 {Value = "hello"});
            var modified = JToken.FromObject(new TestObj2 {Value = null});

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertSinglePatchCall("/Value", "hello", null);
        }

        [Fact]
        public void CreatePatch_TwoEmptyObjects_NoCalls()
        {
            // arrange
            var original = JToken.Parse("{}");
            var modified = JToken.Parse("{}");

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            Assert.Empty(PatchCalls);
        }

        private class TestObj
        {
            public int Value { get; set; }
        }

        private class TestObj2
        {
            public string? Value { get; set; }
        }
    }
}