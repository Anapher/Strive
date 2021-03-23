using PaderConference.Infrastructure.Redis.Tests.Redis;
using PaderConference.Infrastructure.Tests.KeyValue.Scripts.Base;
using PaderConference.IntegrationTests._Helpers;

namespace PaderConference.IntegrationTests.Infrastructure.Redis.Scripts
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
