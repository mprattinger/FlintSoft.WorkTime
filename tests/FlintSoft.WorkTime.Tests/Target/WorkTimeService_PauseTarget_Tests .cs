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
    public class WorkTimeService_PauseTarget_Tests
    {
        private readonly IWorkTimeService _workTimeService;

        public WorkTimeService_PauseTarget_Tests()
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
    }
}
