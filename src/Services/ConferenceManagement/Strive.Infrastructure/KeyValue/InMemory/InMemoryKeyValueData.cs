using System.Collections.Generic;
using Nito.AsyncEx;

namespace Strive.Infrastructure.KeyValue.InMemory
{
    public class InMemoryKeyValueData
    {
        public readonly AsyncReaderWriterLock Lock = new();
        public readonly Dictionary<string, object> Data = new();
    }
}
