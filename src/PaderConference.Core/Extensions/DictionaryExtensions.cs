using System.Collections.Generic;
using System.Linq;

namespace PaderConference.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool EqualItems<T1, T2>(this IReadOnlyDictionary<T1, T2> dictionary1,
            IReadOnlyDictionary<T1, T2> dictionary2) where T1 : notnull
        {
            return dictionary1.OrderBy(kvp => kvp.Key)
                .SequenceEqual(dictionary2.OrderBy(kvp => kvp.Key));
        }
    }
}