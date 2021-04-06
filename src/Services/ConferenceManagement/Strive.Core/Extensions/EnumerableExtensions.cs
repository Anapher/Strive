using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2) where T : notnull
        {
            var cnt = new Dictionary<T, int>();
            foreach (var s in list1)
            {
                if (cnt.TryGetValue(s, out var count))
                    cnt[s] = count + 1;
                else
                    cnt.Add(s, 1);
            }

            foreach (var s in list2)
            {
                if (cnt.TryGetValue(s, out var count))
                    cnt[s] = count - 1;
                else
                    return false;
            }

            return cnt.Values.All(c => c == 0);
        }

        /// <summary>
        ///     Filter the enumerable and only select items that are not null
        /// </summary>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> o) where T : class
        {
            return o.Where(x => x != null)!;
        }
    }
}
