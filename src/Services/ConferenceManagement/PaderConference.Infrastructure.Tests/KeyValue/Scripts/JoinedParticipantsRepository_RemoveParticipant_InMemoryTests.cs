using Microsoft.Extensions.Options;
using PaderConference.Infrastructure.KeyValue;
using PaderConference.Infrastructure.KeyValue.InMemory;
using PaderConference.Infrastructure.Tests.KeyValue.Scripts.Base;

namespace PaderConference.Infrastructure.Tests.KeyValue.Scripts
{
    public class
        JoinedParticipantsRepository_RemoveParticipant_InMemoryTests :
            JoinedParticipantsRepository_RemoveParticipant_Tests
    {
        public JoinedParticipantsRepository_RemoveParticipant_InMemoryTests() : base(
            new InMemoryKeyValueDatabase(new InMemoryKeyValueData(),
                new OptionsWrapper<KeyValueDatabaseOptions>(new KeyValueDatabaseOptions())))
        {
        }
    }
}
