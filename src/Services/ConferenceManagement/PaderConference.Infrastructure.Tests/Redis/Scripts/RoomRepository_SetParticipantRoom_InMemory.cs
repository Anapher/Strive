using PaderConference.Infrastructure.Redis.InMemory;
using PaderConference.Infrastructure.Tests.Redis.Scripts.Base;

namespace PaderConference.Infrastructure.Tests.Redis.Scripts
{
    public class RoomRepository_SetParticipantRoom_InMemory : RoomRepository_SetParticipantRoom_Tests
    {
        public RoomRepository_SetParticipantRoom_InMemory() : base(
            new InMemoryKeyValueDatabase(new InMemoryKeyValueData()))
        {
        }
    }
}
