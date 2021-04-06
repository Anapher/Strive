using System;
using Mongo2Go;

namespace Strive.IntegrationTests
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
