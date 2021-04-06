using System.Collections.Generic;
using System.Collections.Immutable;
using MongoDB.Bson.Serialization.Serializers;

namespace Strive.Infrastructure.Serialization
{
    public class ImmutableListSerializer<TValue> : EnumerableInterfaceImplementerSerializerBase<ImmutableList<TValue>>
    {
        protected override object CreateAccumulator()
        {
            return new List<TValue>();
        }

        protected override ImmutableList<TValue> FinalizeResult(object accumulator)
        {
            return ((List<TValue>) accumulator).ToImmutableList();
        }
    }
}