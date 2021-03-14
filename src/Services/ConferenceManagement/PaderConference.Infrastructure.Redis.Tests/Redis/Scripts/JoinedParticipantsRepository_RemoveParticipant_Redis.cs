using PaderConference.Infrastructure.KeyValue.InMemory;
using PaderConference.Infrastructure.Tests.KeyValue.Scripts.Base;

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
