﻿using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FlintSoft.WorkTime.Tests.Actual
{
    public class WorkTimeService_PrepareCheckins_Tests
    {
        private readonly WorkTimeService _workTimeService;

        public WorkTimeService_PrepareCheckins_Tests()
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
        public void NoTimeNoWorkPauseTime()
        {
            var data = new List<CheckInItem>();

            var res = _workTimeService.PrepareCheckins(data);

            res.worked.Count.Should().Be(0);
            res.paused.Count.Should().Be(0);
        }

        [Fact]
        public void OneTimeNoWorkPauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 6, 11, 0)}
            };

            var res = _workTimeService.PrepareCheckins(data);

            res.worked.Count.Should().Be(0);
            res.paused.Count.Should().Be(0);
        }

        [Fact]
        public void TwoTimesWorkTimeNoPauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 9, 23, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 9, 23, 11, 20, 0)}
            };

            var res = _workTimeService.PrepareCheckins(data);

            res.worked.Count.Should().Be(1);
            res.worked.First().start.Should().Be(new DateTime(2021, 9, 23, 6, 11, 0));
            res.worked.First().end.Should().Be(new DateTime(2021, 9, 23, 11, 20, 0));
            res.paused.Count.Should().Be(0);
        }

        [Fact]
        public void ThreeTimesWorkTimePauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 11, 43, 0)}
            };

            var res = _workTimeService.PrepareCheckins(data);

            res.worked.Count.Should().Be(1);
            res.worked.First().start.Should().Be(new DateTime(2021, 9, 23, 6, 11, 0));
            res.worked.First().end.Should().Be(new DateTime(2021, 9, 23, 11, 20, 0));
            res.paused.Count.Should().Be(1);
            res.paused.First().start.Should().Be(new DateTime(2021, 9, 23, 11, 20, 0));
            res.paused.First().end.Should().Be(new DateTime(2021, 9, 23, 11, 43, 0));
        }

        [Fact]
        public void FourTimesWorkTimePauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 11, 43, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 14, 48, 0)}
            };

            var res = _workTimeService.PrepareCheckins(data);

            res.worked.Count.Should().Be(2);
            res.worked.First().start.Should().Be(new DateTime(2021, 9, 23, 6, 11, 0));
            res.worked.First().end.Should().Be(new DateTime(2021, 9, 23, 11, 20, 0));
            res.worked.Last().start.Should().Be(new DateTime(2021, 9, 23, 11, 43, 0));
            res.worked.Last().end.Should().Be(new DateTime(2021, 9, 23, 14, 48, 0));
            res.paused.Count.Should().Be(1);
            res.paused.First().start.Should().Be(new DateTime(2021, 9, 23, 11, 20, 0));
            res.paused.First().end.Should().Be(new DateTime(2021, 9, 23, 11, 43, 0));
        }
    }
}
