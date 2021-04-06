using Strive.Core.Services.Permissions;
using Xunit;

namespace Strive.Core.Tests.Services.Permissions
{
    public class PermissionsListUtilTests
    {
        [Fact]
        public void TestNotEmpty()
        {
            Assert.NotEmpty(DefinedPermissionsProvider.All);
        }
    }
}
