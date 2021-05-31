using System;
using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public class TagCloudComparer : IEqualityComparer<string>
    {
        private readonly int _shingleSize;
        private readonly double _requiredSimilarity;

        public TagCloudComparer(int shingleSize, double requiredSimilarity)
        {
            _shingleSize = shingleSize;
            _requiredSimilarity = requiredSimilarity;
        }

        public bool Equals(string? x, string? y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;

            if (StringComparer.OrdinalIgnoreCase.Equals(x, y)) return true;

            x = x.ToUpper();
            y = y.ToUpper();

            var xShingles = GetShingles(x, _shingleSize);
            var yShingles = GetShingles(y, _shingleSize);

            var similarity = JaccardSimilarity(xShingles, yShingles);
            return similarity >= _requiredSimilarity;
        }

        public int GetHashCode(string obj)
        {
            return 0;
        }

        public static double JaccardSimilarity(HashSet<string> x, HashSet<string> y)
        {
            return (double) x.Intersect(y).Count() / x.Union(y).Count();
        }

        public static HashSet<string> GetShingles(string s, int shingleSize)
        {
            var result = new HashSet<string>();

            for (var i = 0; i < s.Length - shingleSize; i++)
            {
                result.Add(s.Substring(i, shingleSize));
            }

            return result;
        }
    }
}
