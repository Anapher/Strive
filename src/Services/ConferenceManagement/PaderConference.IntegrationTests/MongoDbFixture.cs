using System;
using Mongo2Go;

namespace PaderConference.IntegrationTests
{
    public class MongoDbFixture : IDisposable
    {
        public MongoDbFixture()
        {
            Runner = MongoDbRunner.Start();
        }

        public void Dispose()
        {
            Runner.Dispose();
        }

        public MongoDbRunner Runner { get; }
    }
}
