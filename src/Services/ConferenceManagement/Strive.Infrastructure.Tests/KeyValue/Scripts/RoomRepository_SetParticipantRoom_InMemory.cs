using Microsoft.Extensions.Options;
using Strive.Infrastructure.KeyValue;
using Strive.Infrastructure.KeyValue.InMemory;
using Strive.Infrastructure.Tests.KeyValue.Scripts.Base;

namespace Strive.Infrastructure.Tests.KeyValue.Scripts
{
    public class RoomRepository_SetParticipantRoom_InMemory : RoomRepository_SetParticipantRoom_Tests
    {
        public RoomRepository_SetParticipantRoom_InMemory() : base(new InMemoryKeyValueDatabase(
            new InMemoryKeyValueData(), new OptionsWrapper<KeyValueDatabaseOptions>(new KeyValueDatabaseOptions())))
        {
        }
    }
}
