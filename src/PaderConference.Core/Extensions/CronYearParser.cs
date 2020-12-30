using System;
using System.Linq;
using Cronos;

namespace PaderConference.Core.Extensions
{
    public static class CronYearParser
    {
        /// <summary>
        ///     Parse the cron expression and get the next occurrence. Support for second field and year field (up to 7 fields with
        ///     automatic detection)
        /// </summary>
        /// <param name="cronExp">The cron expression</param>
        /// <param name="now">The current time</param>
        /// <param name="timeZone">The current time zone</param>
        /// <returns>Return the date and time for the next occurrence or null if there is no next occurrence</returns>
        public static DateTimeOffset? GetNextOccurrence(string cronExp, DateTimeOffset now, TimeZoneInfo timeZone)
        {
            var fragments = cronExp.Split(' ');

            if (fragments.Length == 6)
            {
                var schedule = CronExpression.Parse(cronExp, CronFormat.IncludeSeconds);
                return schedule.GetNextOccurrence(now, timeZone);
            }

            if (fragments.Length == 7)
            {
                // with year
                cronExp = string.Join(' ', fragments.Take(6));
                var schedule = CronExpression.Parse(cronExp, CronFormat.IncludeSeconds);

                var currentStart = now;
                while (true)
                {
                    var nextOccurrence = schedule.GetNextOccurrence(currentStart, timeZone);
                    if (nextOccurrence == null) return null;

                    var nextYear = GetNextYear(fragments[6], nextOccurrence.Value.Year);
                    if (nextYear == null) return null;
                    if (nextYear == nextOccurrence.Value.Year) return nextOccurrence;

                    currentStart = new DateTimeOffset(nextYear.Value, 1, 1, 0, 0, 0, TimeSpan.Zero);
                }
            }

            return CronExpression.Parse(cronExp).GetNextOccurrence(now, timeZone);
        }

        private static int? GetNextYear(string yearExp, int year)
        {
            if (yearExp == "*")
                return year;

            if (yearExp.Contains('/'))
            {
                // Every years(s) starting in
                var split = yearExp.Split('/');
                var startingYear = int.Parse(split[0]);
                var every = int.Parse(split[1]);

                if (startingYear > year) return null;
                if (startingYear == year) return year;

                return startingYear + (int) Math.Ceiling((year - startingYear) / (every * 1.0)) * every;
            }

            if (yearExp.Contains("-"))
            {
                // Every year between and 
                var split = yearExp.Split('-');
                var val1 = int.Parse(split[0]);
                var val2 = int.Parse(split[1]);

                var start = Math.Min(val1, val2);
                var end = Math.Max(val1, val2);

                if (year > end) return null;
                if (start > year) return start;

                return year;
            }

            // specific years
            var specificYears = yearExp.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            if (specificYears.Contains(year)) return year;

            var nextYear = specificYears.Where(x => x > year).OrderBy(x => x).Select(x => (int?) x).FirstOrDefault();
            return nextYear;
        }
    }
}