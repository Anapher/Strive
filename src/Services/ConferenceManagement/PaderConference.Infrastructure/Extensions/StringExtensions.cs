namespace PaderConference.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] Padding = {'='};

        public static string ToUrlBase64(this string s)
        {
            return s.TrimEnd(Padding).Replace('+', '-').Replace('/', '_');
        }
    }
}
