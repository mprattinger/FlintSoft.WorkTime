using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FlintSoft.WorkTime.Tests.Target
{
    public class WorkTimeService_WorkTimeTarget_Tests
    {
        private readonly WorkTimeConfig _cfg;

        public WorkTimeService_WorkTimeTarget_Tests()
        {
            _cfg = new WorkTimeConfig
            {
                WorkDays = new List<WorkTimeDayConfig>() {
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Monday, TargetWorkTime = TimeSpan.FromHours(8.2) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Tuesday, TargetWorkTime = TimeSpan.FromHours(8.2) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Wednesday, TargetWorkTime = TimeSpan.FromHours(8.2) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Thursday, TargetWorkTime = TimeSpan.FromHours(8.2) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Friday, TargetWorkTime = TimeSpan.FromHours(5.7) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Saturday, TargetWorkTime = TimeSpan.Zero },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Sunday, TargetWorkTime = TimeSpan.Zero }
                }
            };
        }

        [Fact]
        public void WorkTimeNormalDay2209ShouldBeWed820()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var res = sut.GetWorkTimeTargetForDay(new DateTime(2021, 9, 22));
            res.Should().Be(TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(12)));
        }

        [Fact]
        public void WorkTimeNormalDay2409ShouldBeFr542()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var res = sut.GetWorkTimeTargetForDay(new DateTime(2021, 9, 24));
            res.Should().Be(TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(42)));
        }

        [Fact]
        public void WorkTimeWeekend2509ShouldBeSa0()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var res = sut.GetWorkTimeTargetForDay(new DateTime(2021, 9, 25));
            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeWeekend2609ShouldBeSo0()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var res = sut.GetWorkTimeTargetForDay(new DateTime(2021, 9, 26));
            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeFeiertag2610ShouldBeTu0()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var res = sut.GetWorkTimeTargetForDay(new DateTime(2021, 10, 26));
            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeFenstertag2510ShouldBeMo0()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var res = sut.GetWorkTimeTargetForDay(new DateTime(2021, 10, 25));
            res.Should().Be(TimeSpan.Zero);
        }

    }
}
