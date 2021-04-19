using System;
using System.Threading.Tasks;
using Strive.Core.Utilities;
using Xunit;

#pragma warning disable 1998

namespace Strive.Core.Tests.Utilities
{
    public class MemorizedTests
    {
        [Fact]
        public async Task Func_ExecuteOnce_ShouldInvokeCallbackAndReturnResult()
        {
            const string result = "Hello!";

            // arrange
            Func<int, ValueTask<string>> func = async i => result;

            // act
            var memorized = Memorized.Func(func);

            // assert
            var actual = await memorized(5);
            Assert.Equal(result, actual);
        }

        [Fact]
        public async Task Func_ExecuteThreeTimesSameParameter_ShouldInvokeCallbackOnceAndReturnEqualResult()
        {
            var counter = 0;

            // arrange
            Func<int, ValueTask<int>> func = async i => ++counter;
            var memorized = Memorized.Func(func);

            // act
            await memorized(5);
            await memorized(5);
            var actual = await memorized(5);

            // assert
            Assert.Equal(1, actual);
            Assert.Equal(1, counter);
        }

        [Fact]
        public async Task Func_ChangeParameter_ShouldEvaluateFunctionAgain()
        {
            var counter = 0;

            // arrange
            Func<int, ValueTask<int>> func = async i => ++counter;
            var memorized = Memorized.Func(func);

            // act
            await memorized(5);
            await memorized(5);

            var actual = await memorized(4);

            // assert

            Assert.Equal(2, actual);
            Assert.Equal(2, counter);
        }
    }
}
