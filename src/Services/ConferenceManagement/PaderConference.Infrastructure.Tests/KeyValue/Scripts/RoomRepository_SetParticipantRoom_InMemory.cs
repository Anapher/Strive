using Microsoft.Extensions.Options;
using PaderConference.Infrastructure.KeyValue;
using PaderConference.Infrastructure.KeyValue.InMemory;
using PaderConference.Infrastructure.Tests.KeyValue.Scripts.Base;

namespace PaderConference.Infrastructure.Tests.KeyValue.Scripts
{
    public class RoomRepository_SetParticipantRoom_InMemory : RoomRepository_SetParticipantRoom_Tests
    {
        public RoomRepository_SetParticipantRoom_InMemory() : base(new InMemoryKeyValueDatabase(
            new InMemoryKeyValueData(), new OptionsWrapper<KeyValueDatabaseOptions>(new KeyValueDatabaseOptions())))
        {
        }
    }
}
