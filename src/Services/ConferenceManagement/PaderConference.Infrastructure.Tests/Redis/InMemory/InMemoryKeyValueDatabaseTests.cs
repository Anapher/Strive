using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Infrastructure.Redis.InMemory;
using Xunit;

namespace PaderConference.Infrastructure.Tests.Redis.InMemory
{
    public class InMemoryKeyValueDatabaseTests
    {
        [Fact]
        public async Task KeySetAsync_WithValidKey_KeyExistsInDictionary()
        {
            const string key = "test";
            const string value = "hello world";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            await database.SetAsync(key, value);

            // assert
            var entry = Assert.Single(data.Data);
            Assert.Equal(key, entry.Key);
            Assert.Equal(value, entry.Value);
        }

        [Fact]
        public async Task KeyGetAsync_KeyExists_ReturnValue()
        {
            const string key = "test";
            const string value = "hello world";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            await database.SetAsync(key, value);
            var result = await database.GetAsync(key);

            // assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task KeyGetAsync_KeyDoesNotExist_ReturnNull()
        {
            const string key = "test";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            var result = await database.GetAsync(key);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task KeyDeleteAsync_KeyDoesNotExist_ReturnFalse()
        {
            const string key = "test";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            var result = await database.KeyDeleteAsync(key);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task KeyDeleteAsync_KeyDoesExist_DeleteAndReturnTrue()
        {
            const string key = "test";
            const string value = "hello world";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            await database.SetAsync(key, value);

            // assert
            var result = await database.KeyDeleteAsync(key);
            Assert.True(result);
            Assert.Empty(data.Data);
        }

        [Fact]
        public async Task HashGetAsync_KeyDoesNotExist_ReturnNull()
        {
            const string key = "test";
            const string field = "field1";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            var result = await database.HashGetAsync(key, field);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task HashGetAsync_KeyDoesExist_ReturnValue()
        {
            const string key = "test";
            const string field = "field1";
            const string value = "hello world";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            await database.HashSetAsync(key, field, value);

            // assert
            var result = await database.HashGetAsync(key, field);
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task HashSetAsync_KeyDoesNotExist_CreateKeyAndAddField()
        {
            const string key = "test";
            const string field = "field1";
            const string value = "hello world";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            await database.HashSetAsync(key, field, value);

            // assert
            Assert.Single(data.Data);
        }

        [Fact]
        public async Task HashSetAsync_KeyDoesExist_AddAnotherField()
        {
            const string key = "test";
            const string field = "field1";
            const string field2 = "field2";
            const string value = "hello world";
            const string value2 = "hello world2";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            await database.HashSetAsync(key, field, value);
            await database.HashSetAsync(key, field2, value2);

            // assert
            var result1 = await database.HashGetAsync(key, field);
            var result2 = await database.HashGetAsync(key, field2);

            Assert.Equal(value, result1);
            Assert.Equal(value2, result2);
        }

        [Fact]
        public async Task HashSetAsync_KeyDoesNotExist_SetMultipleFields()
        {
            const string key = "test";

            var fields = new Dictionary<string, string> {{"field1", "test1"}, {"field2", "test2"}};

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            await database.HashSetAsync(key, fields);

            // assert
            var result = await database.HashGetAllAsync(key);
            AssertKeyValuePairsMatchIgnoreOrder(fields, result);
        }

        [Fact]
        public async Task HashSetAsync_KeyDoesExist_SetMultipleFields()
        {
            const string key = "test";
            const string existingField = "field";
            const string existingFieldValue = "test";

            var fields = new Dictionary<string, string> {{"field1", "test1"}, {"field2", "test2"}};

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            await database.HashSetAsync(key, existingField, existingFieldValue);

            // act
            await database.HashSetAsync(key, fields);

            // assert
            var result = await database.HashGetAllAsync(key);
            var expectedFields =
                fields.Concat(new[] {new KeyValuePair<string, string>(existingField, existingFieldValue)});

            AssertKeyValuePairsMatchIgnoreOrder(expectedFields, result);
        }

        [Fact]
        public async Task HashExistsAsync_KeyDoesNotExist_ReturnFalse()
        {
            const string key = "test";
            const string field = "field";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            var result = await database.HashExistsAsync(key, field);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task HashExistsAsync_KeyExistsButFieldDoesNotExist_ReturnFalse()
        {
            const string key = "test";
            const string field = "field";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            await database.HashSetAsync(key, "randomField", "test");

            // act
            var result = await database.HashExistsAsync(key, field);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task HashDeleteAsync_KeyDoesNotExist_ReturnFalse()
        {
            const string key = "test";
            const string field = "field";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            var result = await database.HashDeleteAsync(key, field);

            // assert
            Assert.False(result);
            Assert.Empty(data.Data);
        }

        [Fact]
        public async Task HashDeleteAsync_KeyExistsButFieldDoesNotExist_ReturnFalse()
        {
            const string key = "test";
            const string field = "field";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            await database.HashSetAsync(key, "randomField", "test");

            // act
            var result = await database.HashDeleteAsync(key, field);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task HashDeleteAsync_SingleFieldExists_DeleteKeyAndReturnTrue()
        {
            const string key = "test";
            const string field = "field";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            await database.HashSetAsync(key, field, "test");

            // act
            var result = await database.HashDeleteAsync(key, field);

            // assert
            Assert.True(result);
            Assert.Empty(data.Data);
        }

        [Fact]
        public async Task HashDeleteAsync_MultipleFieldsExists_PreserveKeyAndReturnTrue()
        {
            const string key = "test";
            const string fieldToDelete = "field";
            const string preservedField = "field2";
            const string preservedFieldValue = "test";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            await database.HashSetAsync(key, fieldToDelete, "test");
            await database.HashSetAsync(key, preservedField, preservedFieldValue);

            // act
            var result = await database.HashDeleteAsync(key, fieldToDelete);

            // assert
            Assert.True(result);

            var preservedFieldExists = await database.HashExistsAsync(key, preservedField);
            Assert.True(preservedFieldExists);

            var fieldToDeleteExists = await database.HashExistsAsync(key, fieldToDelete);
            Assert.False(fieldToDeleteExists);
        }

        [Fact]
        public async Task HashGetAllAsync_KeyDoesNotExist_ReturnEmptyResult()
        {
            const string key = "test";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            var result = await database.HashGetAllAsync(key);

            // assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task HashGetAllAsync_FieldsExist_ReturnFields()
        {
            const string key = "test";
            const string field = "field";
            const string fieldValue = "val";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            await database.HashSetAsync(key, field, fieldValue);

            // act
            var result = await database.HashGetAllAsync(key);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            var expectedResult = new Dictionary<string, string> {{field, fieldValue}};

            AssertKeyValuePairsMatchIgnoreOrder(expectedResult, result);
        }

        [Fact]
        public async Task GetSetAsync_KeyDoesNotExist_SetValueAndReturnNull()
        {
            const string key = "test";
            const string value = "testValue";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            var oldVal = await database.GetSetAsync(key, value);

            // assert
            Assert.Null(oldVal);

            var actualKeyValue = await database.GetAsync(key);
            Assert.Equal(value, actualKeyValue);
        }

        [Fact]
        public async Task GetSetAsync_KeyDoesExist_SetValueAndReturnOldValue()
        {
            const string key = "test";
            const string oldValue = "oldValue";
            const string value = "testValue";

            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            await database.SetAsync(key, oldValue);

            // act
            var actualOldValue = await database.GetSetAsync(key, value);

            // assert
            Assert.Equal(oldValue, actualOldValue);

            var actualKeyValue = await database.GetAsync(key);
            Assert.Equal(value, actualKeyValue);
        }

        [Fact]
        public void CreateTransaction_ExecuteStatements_DontExecuteStatementsBeforeStatement()
        {
            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            using var transaction = database.CreateTransaction();
            _ = transaction.SetAsync("hello", "world");

            // assert
            Assert.Empty(data.Data);
        }

        [Fact]
        public async Task CreateTransaction_ExecuteStatements_ExecuteStatementsOnExecuteAsync()
        {
            // arrange
            var data = new InMemoryKeyValueData();
            var database = new InMemoryKeyValueDatabase(data);

            // act
            using var transaction = database.CreateTransaction();
            _ = transaction.SetAsync("hello", "world");

            await transaction.ExecuteAsync();

            // assert
            Assert.NotEmpty(data.Data);
        }

        private static void AssertKeyValuePairsMatchIgnoreOrder<T>(IEnumerable<KeyValuePair<string, T>> expected,
            IEnumerable<KeyValuePair<string, T>> actual)
        {
            Assert.Equal(expected.OrderBy(x => x.Key), actual.OrderBy(x => x.Key));
        }
    }
}
