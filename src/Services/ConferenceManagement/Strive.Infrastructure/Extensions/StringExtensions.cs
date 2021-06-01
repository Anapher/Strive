using System;

namespace Strive.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] Padding = {'='};

        public static string ToUrlBase64(this string s)
        {
            return s.TrimEnd(Padding).Replace('+', '-').Replace('/', '_');
        }

        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value[1..];
        }

        public static string TrimEnd(this string source, string value,
            StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            if (!source.EndsWith(value))
                return source;

            return source.Remove(source.LastIndexOf(value, comparisonType));
        }
    }
}
