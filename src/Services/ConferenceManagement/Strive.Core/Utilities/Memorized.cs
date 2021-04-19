using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Strive.Core.Utilities
{
    public class Memorized
    {
        public static Func<TSource, ValueTask<TReturn>> Func<TSource, TReturn>(Func<TSource, ValueTask<TReturn>> func)
        {
            KeyValuePair<TSource, TReturn>? cache = null;
            var @lock = new AsyncLock();

            return async s =>
            {
                var tmpCache = cache;
                if (tmpCache.HasValue && Equals(tmpCache.Value.Key, s))
                    return tmpCache.Value.Value;

                using (await @lock.LockAsync())
                {
                    if (!cache.HasValue || !Equals(cache.Value.Key, s))
                        cache = new KeyValuePair<TSource, TReturn>(s, await func(s));

                    return cache.Value.Value;
                }
            };
        }
    }
}
