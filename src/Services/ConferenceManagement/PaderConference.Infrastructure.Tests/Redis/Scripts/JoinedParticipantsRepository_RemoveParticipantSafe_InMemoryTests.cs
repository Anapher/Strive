using PaderConference.Infrastructure.Redis.InMemory;
using PaderConference.Infrastructure.Tests.Redis.Scripts.Base;

namespace PaderConference.Infrastructure.Tests.Redis.Scripts
{
    public class
        JoinedParticipantsRepository_RemoveParticipantSafe_InMemoryTests :
            JoinedParticipantsRepository_RemoveParticipantSafe_Tests
    {
        public JoinedParticipantsRepository_RemoveParticipantSafe_InMemoryTests() : base(
            new InMemoryKeyValueDatabase(new InMemoryKeyValueData()))
        {
        }
    }
}
