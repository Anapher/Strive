using Microsoft.Extensions.Options;
using PaderConference.Infrastructure.KeyValue;
using PaderConference.Infrastructure.KeyValue.InMemory;
using PaderConference.Infrastructure.Tests.KeyValue.Scripts.Base;

namespace PaderConference.Infrastructure.Tests.KeyValue.Scripts
{
    public class
        JoinedParticipantsRepository_RemoveParticipantSafe_InMemoryTests :
            JoinedParticipantsRepository_RemoveParticipantSafe_Tests
    {
        public JoinedParticipantsRepository_RemoveParticipantSafe_InMemoryTests() : base(
            new InMemoryKeyValueDatabase(new InMemoryKeyValueData(),
                new OptionsWrapper<KeyValueDatabaseOptions>(new KeyValueDatabaseOptions())))
        {
        }
    }
}
