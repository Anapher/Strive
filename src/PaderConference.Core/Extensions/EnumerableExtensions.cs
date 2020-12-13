using System.Collections.Generic;
using System.Linq;

namespace PaderConference.Core.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Filter the enumerable and only select items that are not null
        /// </summary>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> o) where T : class
        {
            return o.Where(x => x != null)!;
        }
    }
}
