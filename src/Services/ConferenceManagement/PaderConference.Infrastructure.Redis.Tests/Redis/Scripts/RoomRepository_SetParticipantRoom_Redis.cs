using PaderConference.Infrastructure.Redis.Tests.Redis;
using PaderConference.Infrastructure.Tests.KeyValue.Scripts.Base;
using PaderConference.IntegrationTests._Helpers;
using Xunit;

namespace PaderConference.IntegrationTests.Infrastructure.Redis.Scripts
{
    public class RoomRepository_SetParticipantRoom_Redis : RoomRepository_SetParticipantRoom_Tests,
        IClassFixture<RedisDbConnector>
    {
        public RoomRepository_SetParticipantRoom_Redis(RedisDbConnector connector) : base(
            KeyValueDatabaseFactory.Create(connector.CreateConnection()))
        {
        }
    }
}
