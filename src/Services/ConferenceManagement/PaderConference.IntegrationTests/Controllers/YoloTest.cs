using Xunit;

namespace PaderConference.IntegrationTests.Controllers
{
    public class YoloTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public YoloTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void FooTest()
        {
            var client = _factory.GetClient();

            Assert.Equal(4, 4);
        }
    }
}
