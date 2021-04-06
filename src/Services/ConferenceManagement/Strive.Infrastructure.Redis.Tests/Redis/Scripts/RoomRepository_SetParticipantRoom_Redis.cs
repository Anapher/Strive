using Strive.Infrastructure.Redis.Tests.Redis;
using Strive.Infrastructure.Tests.KeyValue.Scripts.Base;
using Strive.IntegrationTests._Helpers;
using Xunit;

namespace Strive.IntegrationTests.Infrastructure.Redis.Scripts
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
