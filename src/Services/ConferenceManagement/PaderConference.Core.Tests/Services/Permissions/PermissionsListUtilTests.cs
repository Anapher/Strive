using PaderConference.Core.Services.Permissions;
using Xunit;

namespace PaderConference.Core.Tests.Services.Permissions
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
