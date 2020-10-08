using System.Linq;

namespace PaderConference.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1) return char.ToLowerInvariant(str[0]) + str.Substring(1);
            return str;
        }

        public static string ToCamelCasePath(this string path)
        {
            var splits = path.Split('/');
            return string.Join('/', splits.Select(ToCamelCase));
        }
    }
}