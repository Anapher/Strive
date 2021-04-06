using Microsoft.Extensions.Options;
using Strive.Infrastructure.KeyValue;
using Strive.Infrastructure.KeyValue.InMemory;
using Strive.Infrastructure.Tests.KeyValue.Scripts.Base;

namespace Strive.Infrastructure.Tests.KeyValue.Scripts
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
