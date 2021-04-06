using Strive.Infrastructure.Redis.Tests.Redis;
using Strive.Infrastructure.Tests.KeyValue.Scripts.Base;
using Strive.IntegrationTests._Helpers;

namespace Strive.IntegrationTests.Infrastructure.Redis.Scripts
{
    public class
        JoinedParticipantsRepository_RemoveParticipant_Redis : JoinedParticipantsRepository_RemoveParticipant_Tests
    {
        public JoinedParticipantsRepository_RemoveParticipant_Redis(RedisDbConnector connector) : base(
            KeyValueDatabaseFactory.Create(connector.CreateConnection()))
        {
        }
    }
}
