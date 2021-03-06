using Xunit;

namespace PaderConference.IntegrationTests
{
    [CollectionDefinition(Definition)]
    public class IntegrationTestCollection : ICollectionFixture<MongoDbFixture>
    {
        public const string Definition = "Integration Tests";
    }
}
