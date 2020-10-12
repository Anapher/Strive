using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace PaderConference.Infrastructure.Utilities
{
    public class ConcurrentMap<T1, T2> where T1 : notnull where T2 : notnull
    {
        private readonly ConcurrentDictionary<T1, T2> _forward = new ConcurrentDictionary<T1, T2>();
        private readonly ConcurrentDictionary<T2, T1> _reverse = new ConcurrentDictionary<T2, T1>();

        public ConcurrentMap()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public Indexer<T1, T2> Forward { get; }
        public Indexer<T2, T1> Reverse { get; }

        public bool TryAdd(T1 t1, T2 t2)
        {
            if (_forward.TryAdd(t1, t2))
                if (!_reverse.TryAdd(t2, t1))
                {
                    _forward.TryRemove(t1, out _);
                    return false;
                }
                else
                {
                    return true;
                }

            return false;
        }

        public bool TryRemove(T1 t1)
        {
            if (_forward.TryRemove(t1, out var existingT2))
                if (!_reverse.TryRemove(existingT2, out _))
                {
                    _forward.TryAdd(t1, existingT2);
                    return false;
                }
                else
                {
                    return true;
                }

            return false;
        }

        public class Indexer<T3, T4> where T3 : notnull
        {
            private readonly ConcurrentDictionary<T3, T4> _dictionary;

            public Indexer(ConcurrentDictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            public T4 this[T3 index] => _dictionary[index];

            public bool TryGetValue(T3 key, [MaybeNullWhen(false)] out T4 value)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }
    }
}