using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Permissions;
using Xunit;

namespace PaderConference.Core.Tests.Services.Permissions
{
    public class PermissionStackTests
    {
        private readonly PermissionDescriptor<bool> _testDescriptor1 = new("test");
        private readonly PermissionDescriptor<int> _testDescriptor2 = new("test2");

        [Fact]
        public async Task TestRespectOrder()
        {
            // arrange
            var layers = new List<IReadOnlyDictionary<string, JValue>>
            {
                new[] {_testDescriptor1.Configure(true)}.ToImmutableDictionary(),
                new[] {_testDescriptor1.Configure(false)}.ToImmutableDictionary(),
            };

            var stack = new PermissionStack(layers);

            // act
            var permission = await stack.GetPermission(_testDescriptor1);

            // assert
            Assert.False(permission);
        }

        [Fact]
        public async Task TestPermissionDoesNotExist()
        {
            // arrange
            var layers = new List<IReadOnlyDictionary<string, JValue>>
            {
                new[] {_testDescriptor1.Configure(true)}.ToImmutableDictionary(),
            };

            var stack = new PermissionStack(layers);

            // act
            var permission = await stack.GetPermission(_testDescriptor2);

            // assert
            Assert.Equal(0, permission);
        }

        [Fact]
        public void TestPermissionStackFlatten()
        {
            // arrange
            var layers = new List<IReadOnlyDictionary<string, JValue>>
            {
                new[] {_testDescriptor1.Configure(true), _testDescriptor2.Configure(45)}.ToImmutableDictionary(),
                new[] {_testDescriptor1.Configure(false)}.ToImmutableDictionary(),
            };

            var stack = new PermissionStack(layers);

            // act
            var flatten = stack.Flatten();

            // assert
            Assert.Equal(2, flatten.Count);
            Assert.False(flatten[_testDescriptor1.Key].Value<bool>());
            Assert.Equal(45, flatten[_testDescriptor2.Key].Value<int>());
        }
    }
}
