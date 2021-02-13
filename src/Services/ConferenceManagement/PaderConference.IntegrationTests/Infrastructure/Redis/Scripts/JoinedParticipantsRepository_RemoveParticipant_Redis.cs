using PaderConference.Infrastructure.Redis.InMemory;
using PaderConference.Infrastructure.Tests.Redis.Scripts;

namespace PaderConference.IntegrationTests.Infrastructure.Redis.Scripts
{
    public class
        JoinedParticipantsRepository_RemoveParticipant_Redis : JoinedParticipantsRepository_RemoveParticipant_Tests
    {
        public JoinedParticipantsRepository_RemoveParticipant_Redis() : base(
            new InMemoryKeyValueDatabase(new InMemoryKeyValueData()))
        {
        }
    }
}
