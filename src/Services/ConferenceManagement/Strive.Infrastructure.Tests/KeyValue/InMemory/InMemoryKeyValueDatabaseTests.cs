using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Strive.Infrastructure.KeyValue;
using Strive.Infrastructure.KeyValue.InMemory;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Infrastructure.Tests.KeyValue.InMemory
{
    public class InMemoryKeyValueDatabaseTests
    {
        private readonly InMemoryKeyValueData _data = new();
        private readonly InMemoryKeyValueDatabase _database;

        public InMemoryKeyValueDatabaseTests()
        {
            _database = new InMemoryKeyValueDatabase(_data,
                new OptionsWrapper<KeyValueDatabaseOptions>(new KeyValueDatabaseOptions()));
        }

        [Fact]
        public async Task KeySetAsync_WithValidKey_KeyExistsInDictionary()
        {
            const string key = "test";
            const string value = "hello world";

            // act
            await _database.SetAsync(key, value);

            // assert
            var entry = Assert.Single(_data.Data);
            Assert.Equal(key, entry.Key);
            Assert.Equal(value, entry.Value);
        }

        [Fact]
        public async Task KeyGetAsync_KeyExists_ReturnValue()
        {
            const string key = "test";
            const string value = "hello world";

            // act
            await _database.SetAsync(key, value);
            var result = await _database.GetAsync(key);

            // assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task KeyGetAsync_KeyDoesNotExist_ReturnNull()
        {
            const string key = "test";

            // act
            var result = await _database.GetAsync(key);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task KeyDeleteAsync_KeyDoesNotExist_ReturnFalse()
        {
            const string key = "test";

            // act
            var result = await _database.KeyDeleteAsync(key);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task KeyDeleteAsync_KeyDoesExist_DeleteAndReturnTrue()
        {
            const string key = "test";
            const string value = "hello world";

            // act
            await _database.SetAsync(key, value);

            // assert
            var result = await _database.KeyDeleteAsync(key);
            Assert.True(result);
            Assert.Empty(_data.Data);
        }

        [Fact]
        public async Task HashGetAsync_KeyDoesNotExist_ReturnNull()
        {
            const string key = "test";
            const string field = "field1";

            // act
            var result = await _database.HashGetAsync(key, field);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task HashGetAsync_KeyDoesExist_ReturnValue()
        {
            const string key = "test";
            const string field = "field1";
            const string value = "hello world";

            // act
            await _database.HashSetAsync(key, field, value);

            // assert
            var result = await _database.HashGetAsync(key, field);
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task HashSetAsync_KeyDoesNotExist_CreateKeyAndAddField()
        {
            const string key = "test";
            const string field = "field1";
            const string value = "hello world";

            // act
            await _database.HashSetAsync(key, field, value);

            // assert
            Assert.Single(_data.Data);
        }

        [Fact]
        public async Task HashSetAsync_KeyDoesExist_AddAnotherField()
        {
            const string key = "test";
            const string field = "field1";
            const string field2 = "field2";
            const string value = "hello world";
            const string value2 = "hello world2";

            // act
            await _database.HashSetAsync(key, field, value);
            await _database.HashSetAsync(key, field2, value2);

            // assert
            var result1 = await _database.HashGetAsync(key, field);
            var result2 = await _database.HashGetAsync(key, field2);

            Assert.Equal(value, result1);
            Assert.Equal(value2, result2);
        }

        [Fact]
        public async Task HashSetAsync_KeyDoesNotExist_SetMultipleFields()
        {
            const string key = "test";

            var fields = new Dictionary<string, string> {{"field1", "test1"}, {"field2", "test2"}};

            // act
            await _database.HashSetAsync(key, fields);

            // assert
            var result = await _database.HashGetAllAsync(key);
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
            await _database.HashSetAsync(key, existingField, existingFieldValue);

            // act
            await _database.HashSetAsync(key, fields);

            // assert
            var result = await _database.HashGetAllAsync(key);
            var expectedFields =
                fields.Concat(new[] {new KeyValuePair<string, string>(existingField, existingFieldValue)});

            AssertKeyValuePairsMatchIgnoreOrder(expectedFields, result);
        }

        [Fact]
        public async Task HashExistsAsync_KeyDoesNotExist_ReturnFalse()
        {
            const string key = "test";
            const string field = "field";

            // act
            var result = await _database.HashExistsAsync(key, field);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task HashExistsAsync_KeyExistsButFieldDoesNotExist_ReturnFalse()
        {
            const string key = "test";
            const string field = "field";

            await _database.HashSetAsync(key, "randomField", "test");

            // act
            var result = await _database.HashExistsAsync(key, field);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task HashDeleteAsync_KeyDoesNotExist_ReturnFalse()
        {
            const string key = "test";
            const string field = "field";

            // act
            var result = await _database.HashDeleteAsync(key, field);

            // assert
            Assert.False(result);
            Assert.Empty(_data.Data);
        }

        [Fact]
        public async Task HashDeleteAsync_KeyExistsButFieldDoesNotExist_ReturnFalse()
        {
            const string key = "test";
            const string field = "field";

            await _database.HashSetAsync(key, "randomField", "test");

            // act
            var result = await _database.HashDeleteAsync(key, field);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task HashDeleteAsync_SingleFieldExists_DeleteKeyAndReturnTrue()
        {
            const string key = "test";
            const string field = "field";

            await _database.HashSetAsync(key, field, "test");

            // act
            var result = await _database.HashDeleteAsync(key, field);

            // assert
            Assert.True(result);
            Assert.Empty(_data.Data);
        }

        [Fact]
        public async Task HashDeleteAsync_MultipleFieldsExists_PreserveKeyAndReturnTrue()
        {
            const string key = "test";
            const string fieldToDelete = "field";
            const string preservedField = "field2";
            const string preservedFieldValue = "test";

            // arrange
            await _database.HashSetAsync(key, fieldToDelete, "test");
            await _database.HashSetAsync(key, preservedField, preservedFieldValue);

            // act
            var result = await _database.HashDeleteAsync(key, fieldToDelete);

            // assert
            Assert.True(result);

            var preservedFieldExists = await _database.HashExistsAsync(key, preservedField);
            Assert.True(preservedFieldExists);

            var fieldToDeleteExists = await _database.HashExistsAsync(key, fieldToDelete);
            Assert.False(fieldToDeleteExists);
        }

        [Fact]
        public async Task HashGetAllAsync_KeyDoesNotExist_ReturnEmptyResult()
        {
            const string key = "test";

            // act
            var result = await _database.HashGetAllAsync(key);

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
            await _database.HashSetAsync(key, field, fieldValue);

            // act
            var result = await _database.HashGetAllAsync(key);

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

            // act
            var oldVal = await _database.GetSetAsync(key, value);

            // assert
            Assert.Null(oldVal);

            var actualKeyValue = await _database.GetAsync(key);
            Assert.Equal(value, actualKeyValue);
        }

        [Fact]
        public async Task GetSetAsync_KeyDoesExist_SetValueAndReturnOldValue()
        {
            const string key = "test";
            const string oldValue = "oldValue";
            const string value = "testValue";

            // arrange
            await _database.SetAsync(key, oldValue);

            // act
            var actualOldValue = await _database.GetSetAsync(key, value);

            // assert
            Assert.Equal(oldValue, actualOldValue);

            var actualKeyValue = await _database.GetAsync(key);
            Assert.Equal(value, actualKeyValue);
        }

        [Fact]
        public async Task ListRightPushAsync_KeyDoesNotExist_CreateListAndAddItem()
        {
            const string key = "test";
            const string value = "testValue";

            // act
            await _database.ListRightPushAsync(key, value);

            // assert
            var entry = Assert.Single(_data.Data);
            Assert.Equal(key, entry.Key);
            var list = Assert.IsType<List<string>>(entry.Value);
            Assert.Equal(value, Assert.Single(list));
        }

        [Fact]
        public async Task ListRightPushAsync_ListAlreadyContainsItem_AddItemToEndOfList()
        {
            const string key = "test";
            const string initialValue = "init";
            const string value = "testValue";

            // arrange
            await _database.ListRightPushAsync(key, initialValue);

            // act
            await _database.ListRightPushAsync(key, value);

            // assert
            var entry = Assert.Single(_data.Data);
            var list = Assert.IsType<List<string>>(entry.Value);
            Assert.Equal(new[] {initialValue, value}, list);
        }

        [Fact]
        public async Task ListLenAsync_KeyDoesNotExist_ReturnZero()
        {
            const string key = "test";

            // act
            var actual = await _database.ListLenAsync(key);

            // assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public async Task ListLenAsync_ListExists_ReturnListCount()
        {
            const string key = "test";

            await _database.ListRightPushAsync(key, "test1");
            await _database.ListRightPushAsync(key, "test2");

            // act
            var actual = await _database.ListLenAsync(key);

            // assert
            Assert.Equal(2, actual);
        }

        [Fact]
        public async Task ListRangeAsync_KeyDoesNotExist_ReturnEmptyCollection()
        {
            const string key = "test";

            // act
            var actual = await _database.ListRangeAsync(key, 0, -1);

            // assert
            Assert.Empty(actual);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, -1)]
        [InlineData(0, 200)]
        [InlineData(-1, 0)]
        public async Task ListRangeAsync_SingleItemExists_ReturnThisItem(int start, int end)
        {
            const string key = "test";
            const string item = "testItem";

            await _database.ListRightPushAsync(key, item);

            // act
            var actual = await _database.ListRangeAsync(key, start, end);

            // assert
            Assert.Equal(item, Assert.Single(actual));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 100)]
        public async Task ListRangeAsync_SingleItemExists_ReturnEmptyCollection(int start, int end)
        {
            const string key = "test";
            const string item = "testItem";

            await _database.ListRightPushAsync(key, item);

            // act
            var actual = await _database.ListRangeAsync(key, start, end);

            // assert
            Assert.Empty(actual);
        }

        [Theory]
        [InlineData(1, 1, new[] {"item1"})]
        [InlineData(1, 2, new[] {"item1", "item2"})]
        [InlineData(0, 2, new[] {"item0", "item1", "item2"})]
        [InlineData(-2, -1, new[] {"item8", "item9"})]
        [InlineData(8, 100, new[] {"item8", "item9"})]
        [InlineData(6, -1, new[] {"item6", "item7", "item8", "item9"})]
        [InlineData(10, 11, new string[0])]
        public async Task ListRangeAsync_MultipleItems_CorrectlyReturnRange(int start, int end, string[] expected)
        {
            const string key = "test";

            for (var i = 0; i < 10; i++)
            {
                await _database.ListRightPushAsync(key, $"item{i}");
            }

            // act
            var actual = await _database.ListRangeAsync(key, start, end);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SetAddAsync_SetDoesNotExist_CreateSetAndReturnTrue()
        {
            const string key = "test";
            const string value = "test1";

            // act
            var result = await _database.SetAddAsync(key, value);

            // assert
            Assert.True(result);
            Assert.NotEmpty(_data.Data);
        }

        [Fact]
        public async Task SetAddAsync_SetDoesExist_AddValueToExistingSet()
        {
            const string key = "test";
            const string value1 = "test1";
            const string value2 = "test2";

            // arrange
            await _database.SetAddAsync(key, value1);

            // act
            var result = await _database.SetAddAsync(key, value2);

            // assert
            Assert.True(result);

            var actualSet = await _database.SetMembersAsync(key);
            AssertHelper.AssertScrambledEquals(new[] {value1, value2}, actualSet);
        }

        [Fact]
        public async Task SetAddAsync_ItemAlreadyExists_ReturnFalse()
        {
            const string key = "test";
            const string value1 = "test1";

            await _database.SetAddAsync(key, value1);

            // act
            var result = await _database.SetAddAsync(key, value1);

            // assert
            Assert.False(result);

            var actualSet = await _database.SetMembersAsync(key);
            AssertHelper.AssertScrambledEquals(new[] {value1}, actualSet);
        }

        [Fact]
        public async Task SetRemoveAsync_SetDoesNotExist_ReturnFalse()
        {
            const string key = "test";

            // act
            var result = await _database.SetRemoveAsync(key, "test");

            // assert
            Assert.False(result);
            Assert.Empty(_data.Data);
        }

        [Fact]
        public async Task SetRemoveAsync_LastItem_ReturnTrueAndDeleteKey()
        {
            const string key = "test";
            const string item = "item";

            await _database.SetAddAsync(key, item);

            // act
            var result = await _database.SetRemoveAsync(key, item);

            // assert
            Assert.True(result);
            Assert.Empty(_data.Data);
        }

        [Fact]
        public async Task SetRemoveAsync_ItemExists_ReturnTrueAndDeleteItem()
        {
            const string key = "test";
            const string item1 = "item1";
            const string item2 = "item2";

            // arrange
            await _database.SetAddAsync(key, item1);
            await _database.SetAddAsync(key, item2);

            // act
            var result = await _database.SetRemoveAsync(key, item1);

            // assert
            Assert.True(result);

            var items = await _database.SetMembersAsync(key);
            AssertHelper.AssertScrambledEquals(new[] {item2}, items);
        }

        [Fact]
        public async Task SetMembersAsync_KeyDoesNotExist_ReturnEmptyList()
        {
            const string key = "test";

            // act
            var result = await _database.SetMembersAsync(key);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task SetMembersAsync_MembersExist_ReturnList()
        {
            const string key = "test";
            const string value = "value";

            await _database.SetAddAsync(key, value);

            // act
            var result = await _database.SetMembersAsync(key);

            // assert
            var actualValue = Assert.Single(result);
            Assert.Equal(value, actualValue);
        }

        [Fact]
        public void CreateTransaction_ExecuteStatements_DontExecuteStatementsBeforeStatement()
        {
            // act
            using var transaction = _database.CreateTransaction();
            _ = transaction.SetAsync("hello", "world");

            // assert
            Assert.Empty(_data.Data);
        }

        [Fact]
        public async Task CreateTransaction_ExecuteStatements_ExecuteStatementsOnExecuteAsync()
        {
            // act
            using var transaction = _database.CreateTransaction();
            _ = transaction.SetAsync("hello", "world");

            await transaction.ExecuteAsync();

            // assert
            Assert.NotEmpty(_data.Data);
        }

        private static void AssertKeyValuePairsMatchIgnoreOrder<T>(IEnumerable<KeyValuePair<string, T>> expected,
            IEnumerable<KeyValuePair<string, T>> actual)
        {
            Assert.Equal(expected.OrderBy(x => x.Key), actual.OrderBy(x => x.Key));
        }
    }
}
