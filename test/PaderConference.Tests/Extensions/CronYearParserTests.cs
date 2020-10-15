using System;
using System.Globalization;
using PaderConference.Extensions;
using Xunit;

namespace PaderConference.Tests.Extensions
{
    public class CronYearParserTests
    {
        private static readonly DateTimeOffset _now = new DateTimeOffset(2020, 7, 1, 0, 0, 0, TimeSpan.Zero);
        private static readonly TimeZoneInfo _timeZone = TimeZoneInfo.Utc;

        [Theory]
        // without year
        [InlineData("0 0 16 ? * MON", "07/06/2020 16:00")]
        [InlineData("0 16 ? * MON", "07/06/2020 16:00")]

        // every year
        [InlineData("0 0 16 ? * MON *", "07/06/2020 16:00")]

        // every few years
        [InlineData("0 0 16 ? * MON 2019/3", "01/03/2022 16:00")] // next time in a few years
        [InlineData("0 0 16 ? * MON 2020/3", "07/06/2020 16:00")] // is this year
        [InlineData("0 0 16 ? * MON 2020/3", "01/02/2023 16:00",
            "12/31/2020 17:00")] // is this year but next occurrence would be next year
        [InlineData("0 0 16 ? * MON 2020/1", "01/04/2021 16:00",
            "12/31/2020 17:00")] // every year

        // specific years
        [InlineData("0 0 16 ? * MON 2031", "01/06/2031 16:00")] // only 2031
        [InlineData("0 0 16 ? * MON 2011,2017,2021", "01/04/2021 16:00")] // only some years
        [InlineData("0 0 16 ? * MON 2011,2019,2020,2036", "07/06/2020 16:00")]
        [InlineData("0 0 16 ? * MON 2011,2015", null)]

        // years between
        [InlineData("0 0 16 ? * MON 2015-2030", "07/06/2020 16:00")] // 2015-2030
        [InlineData("0 0 16 ? * MON 2021-2030", "01/04/2021 16:00")] // 2021-2030
        [InlineData("0 0 16 ? * MON 2016-2019", null)] // 2021-2030
        public void TestParse(string cronExp, string? nextOccurrence, string? nowStr = null)
        {
            var now = nowStr == null ? _now : DateTimeOffset.Parse(nowStr, CultureInfo.InvariantCulture);

            var next = CronYearParser.GetNextOccurrence(cronExp, now, _timeZone);
            Assert.Equal(nextOccurrence, next?.ToString("g", CultureInfo.InvariantCulture));
        }
    }
}