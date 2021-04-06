using System.Linq;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.InMemory;
using Xunit;

namespace Strive.Infrastructure.Tests.KeyValue.InMemory
{
    public class InMemoryTransactionTests
    {
        [Fact]
        public void Test_ThatAllOperationsAreOverwritten()
        {
            var methodsThatMustBeOverwritten = typeof(IKeyValueDatabaseActions).GetMethods();

            var transactionType = typeof(InMemoryDatabaseTransaction);
            foreach (var expectedMethod in methodsThatMustBeOverwritten)
            {
                var actualMethod = transactionType.GetMethod(expectedMethod.Name,
                    expectedMethod.GetParameters().Select(x => x.ParameterType).ToArray());

                if (transactionType != actualMethod?.DeclaringType)
                    AssertFail(
                        $"The method {expectedMethod} declared in {typeof(IKeyValueDatabaseActions)} is not overwritten in {typeof(InMemoryDatabaseTransaction)}. This will lead to issues as the calls are immediately executed by {typeof(InMemoryDatabaseActions)} instead of being cached by the {typeof(InMemoryDatabaseTransaction)}.\r\nTo fix this, please overwrite the method.");
            }
        }

        private void AssertFail(string message)
        {
            Assert.True(false, message);
        }
    }
}
