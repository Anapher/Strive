using Strive.Infrastructure.Redis.Tests.Redis;
using Strive.Infrastructure.Tests.KeyValue.Scripts.Base;
using Strive.IntegrationTests._Helpers;
using Xunit;

namespace Strive.IntegrationTests.Infrastructure.Redis.Scripts
{
    public class JoinedParticipantsRepository_RemoveParticipantSafe_Redis :
        JoinedParticipantsRepository_RemoveParticipantSafe_Tests, IClassFixture<RedisDbConnector>
    {
        public JoinedParticipantsRepository_RemoveParticipantSafe_Redis(RedisDbConnector connector) : base(
            KeyValueDatabaseFactory.Create(connector.CreateConnection()))
        {
        }
    }
}
