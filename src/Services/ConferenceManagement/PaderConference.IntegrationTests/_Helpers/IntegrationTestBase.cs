using System.Net.Http;
using Serilog;
using Serilog.Core;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests._Helpers
{
    public abstract class IntegrationTestBase
    {
        protected readonly CustomWebApplicationFactory Factory;
        protected readonly Logger Logger;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb)
        {
            Factory = new CustomWebApplicationFactory(mongoDb, testOutputHelper);
            Logger = testOutputHelper.CreateTestLogger();
            Client = Factory.CreateClient();
        }
    }
}
