using System;
using Strive.Core.Domain.Entities;
using Strive.Core.Services.ConferenceControl;
using Xunit;

namespace Strive.Core.Tests.Services.ConferenceControl
{
    public class ConferenceSchedulerTests
    {
        [Fact]
        public void TestGetNextExecutionFixedStartTime()
        {
            // arrange
            var startTime = new DateTimeOffset(2020, 10, 5, 0, 0, 0, TimeSpan.Zero);
            var now = new DateTimeOffset(2020, 10, 4, 0, 0, 0, TimeSpan.Zero);
            var timezone = TimeZoneInfo.Utc;

            // act
            var next = ScheduleUtils.GetNextExecution(new ConferenceConfiguration {StartTime = startTime}, now,
                timezone);

            // assert
            Assert.Equal(startTime, next);
        }

        [Fact]
        public void TestGetNextExecutionStartTimeInFutureCron()
        {
            // arrange
            var startTime = new DateTimeOffset(2020, 10, 15, 0, 0, 0, TimeSpan.Zero);
            var now = new DateTimeOffset(2020, 10, 4, 0, 0, 0, TimeSpan.Zero);
            var timezone = TimeZoneInfo.Utc;

            var cron = "0 0 16 ? * MON *";

            // act
            var next = ScheduleUtils.GetNextExecution(
                new ConferenceConfiguration {StartTime = startTime, ScheduleCron = cron}, now, timezone);

            // assert
            Assert.Equal(startTime, next);
        }

        [Fact]
        public void TestGetNextExecutionCron()
        {
            // arrange
            var startTime = new DateTimeOffset(2020, 10, 15, 0, 0, 0, TimeSpan.Zero);
            var now = new DateTimeOffset(2020, 10, 15, 0, 0, 0, TimeSpan.Zero);
            var timezone = TimeZoneInfo.Utc;

            var cron = "0 0 16 ? * MON *";

            // act
            var next = ScheduleUtils.GetNextExecution(
                new ConferenceConfiguration {StartTime = startTime, ScheduleCron = cron}, now, timezone);

            // assert
            var nextCron = new DateTimeOffset(2020, 10, 19, 16, 0, 0, TimeSpan.Zero);
            Assert.Equal(nextCron, next);
        }
    }
}