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
        private readonly IWorkTimeService _workTimeService;

        public WorkTimeService_WorkTimeTarget_Tests()
        {
            var cfg = new WorkTimeConfig
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

            _workTimeService = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), cfg);
        }

        [Fact]
        public void WorkTimeNormalDay2209ShouldBeWed820()
        {
            var res = _workTimeService.GetWorkTimeTargetForDay(new DateTime(2021, 9, 22));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Wednesday);
            res.TargetWorkTime.Should().Be(TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(12)));
        }

        [Fact]
        public void WorkTimeNormalDay2409ShouldBeFr542()
        {
            var res = _workTimeService.GetWorkTimeTargetForDay(new DateTime(2021, 9, 24));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Friday);
            res.TargetWorkTime.Should().Be(TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(42)));
        }

        [Fact]
        public void WorkTimeWeekend2509ShouldBeSa0()
        {
            var res = _workTimeService.GetWorkTimeTargetForDay(new DateTime(2021, 9, 25));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Saturday);
            res.TargetWorkTime.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeWeekend2609ShouldBeSo0()
        {
            var res = _workTimeService.GetWorkTimeTargetForDay(new DateTime(2021, 9, 26));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Sunday);
            res.TargetWorkTime.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeFeiertag2610ShouldBeTu0()
        {
            var res = _workTimeService.GetWorkTimeTargetForDay(new DateTime(2021, 10, 26));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Tuesday);
            res.TargetWorkTime.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeFenstertag2510ShouldBeMo0()
        {
            var res = _workTimeService.GetWorkTimeTargetForDay(new DateTime(2021, 10, 25));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Monday);
            res.TargetWorkTime.Should().Be(TimeSpan.Zero);
        }

    }
}
