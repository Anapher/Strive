using PaderConference.Infrastructure.Redis.InMemory;

namespace PaderConference.Infrastructure.Tests.Redis.Scripts
{
    public class
        JoinedParticipantsRepository_RemoveParticipant_InMemoryTests :
            JoinedParticipantsRepository_RemoveParticipant_Tests
    {
        public JoinedParticipantsRepository_RemoveParticipant_InMemoryTests() : base(
            new InMemoryKeyValueDatabase(new InMemoryKeyValueData()))
        {
        }
    }
}
