using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FlintSoft.WorkTime.Tests
{
    public class WorkTimeServiceTests
    {
        private readonly IWorkTimeService _workTimeService;

        public WorkTimeServiceTests()
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

        #region PauseTargetTests
        [Fact]
        public void PauseTarget30MinWhenMoreThen6()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(12)));
            res.Should().Be(TimeSpan.FromMinutes(30));
        }

        [Fact]
        public void PauseTarget30MinWhen6()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(6));
            res.Should().Be(TimeSpan.FromMinutes(30));
        }

        [Fact]
        public void PauseTarget0MinWhenLessThen6AndNoFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(5));
            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void PauseTarget0MinWhen0AndNoFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.Zero);
            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void PauseTarget0MinWhenLessThen6AndFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(5), true);
            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void PauseTarget0MinWhen0AndFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.Zero, true);
            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void PauseTarget15MinWhenBtw6And615AndFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(6).Add(TimeSpan.FromMinutes(13)), true);
            res.Should().Be(TimeSpan.FromMinutes(15));
        }

        [Fact]
        public void PauseTarget15MinWhenBtw6And61AndFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(6).Add(TimeSpan.FromMinutes(1)), true);
            res.Should().Be(TimeSpan.FromMinutes(15));
        }

        [Fact]
        public void PauseTarget17MinWhen617AndFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(6).Add(TimeSpan.FromMinutes(17)), true);
            res.Should().Be(TimeSpan.FromMinutes(17));
        }

        [Fact]
        public void PauseTarget30MinWhen630AndFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(6).Add(TimeSpan.FromMinutes(30)), true);
            res.Should().Be(TimeSpan.FromMinutes(30));
        }

        [Fact]
        public void PauseTarget30MinWhenMore630AndFriday()
        {
            var res = _workTimeService.GetTargetPauseForTimeSpan(TimeSpan.FromHours(6).Add(TimeSpan.FromMinutes(45)), true);
            res.Should().Be(TimeSpan.FromMinutes(30));
        }
        #endregion

        #region WorkTimeForDay
        [Fact]
        public void WorkTimeNormalDay2209ShouldBeWed820()
        {
            var res = _workTimeService.GetWorkTimeForDay(new DateTime(2021, 9, 22));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Wednesday);
            res.TargetWorkTime.Should().Be(TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(12)));
        }

        [Fact]
        public void WorkTimeNormalDay2409ShouldBeFr542()
        {
            var res = _workTimeService.GetWorkTimeForDay(new DateTime(2021, 9, 24));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Friday);
            res.TargetWorkTime.Should().Be(TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(42)));
        }

        [Fact]
        public void WorkTimeWeekend2509ShouldBeSa0()
        {
            var res = _workTimeService.GetWorkTimeForDay(new DateTime(2021, 9, 25));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Saturday);
            res.TargetWorkTime.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeWeekend2609ShouldBeSo0()
        {
            var res = _workTimeService.GetWorkTimeForDay(new DateTime(2021, 9, 26));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Sunday);
            res.TargetWorkTime.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeFeiertag2610ShouldBeTu0()
        {
            var res = _workTimeService.GetWorkTimeForDay(new DateTime(2021, 10, 26));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Tuesday);
            res.TargetWorkTime.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void WorkTimeFenstertag2510ShouldBeMo0()
        {
            var res = _workTimeService.GetWorkTimeForDay(new DateTime(2021, 10, 25));
            res.Should().BeOfType<WorkTimeDayConfig>();
            res.WorkDay.Should().Be(DayOfWeek.Monday);
            res.TargetWorkTime.Should().Be(TimeSpan.Zero);
        }
        #endregion
    }
}
