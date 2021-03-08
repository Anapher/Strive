using JsonPatchGenerator.Handlers;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JsonPatchGenerator.Tests.Handlers
{
    public class ArrayPatchTypeHandlerTests : TypeHandlerTestBase
    {
        private readonly ArrayPatchTypeHandler _handler = new();

        [Fact]
        public void CreatePatch_TwoEmptyArrays_NoOperations()
        {
            // arrange
            var original = JToken.Parse("[]");
            var modified = JToken.Parse("[]");

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            Assert.Empty(Document.Operations);
        }

        [Fact]
        public void CreatePatch_AddItem_AddOperation()
        {
            // arrange
            var original = JToken.Parse("[]");
            var modified = JToken.Parse("[5]");

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertAddOperation("/-", 5);
            Assert.Single(Document.Operations);
        }

        [Fact]
        public void CreatePatch_Remove_AddOperation()
        {
            // arrange
            var original = JToken.Parse("[5]");
            var modified = JToken.Parse("[]");

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertRemoveOperation("/0");
            Assert.Single(Document.Operations);
        }

        [Fact]
        public void CreatePatch_SwapItems_MoveOperation()
        {
            // arrange
            var original = JToken.Parse("[4,5]");
            var modified = JToken.Parse("[5,4]");

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertMoveOperation("/1", "/0");
        }

        [Fact]
        public void CreatePatch_AddItemsToMiddle_AddOperation()
        {
            // arrange
            var original = JToken.Parse("[1,2,3,5]");
            var modified = JToken.Parse("[1,2,3,4,5]");

            // act
            _handler.CreatePatch(original, modified, JsonPatchPath.Root, Context);

            // assert
            AssertAddOperation("/3", 4);
        }

        private void AssertAddOperation(string path, object item)
        {
            Assert.Contains(Document.Operations,
                operation => operation.OperationType == OperationType.Add && operation.path == path &&
                             JToken.DeepEquals((JToken) operation.value, JToken.FromObject(item)));
        }

        private void AssertRemoveOperation(string path)
        {
            Assert.Contains(Document.Operations,
                operation => operation.OperationType == OperationType.Remove && operation.path == path);
        }

        private void AssertMoveOperation(string from, string to)
        {
            Assert.Contains(Document.Operations,
                operation => operation.OperationType == OperationType.Move && operation.from == from &&
                             operation.path == to);
        }
    }
}