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
    public class WorkTimeService_ActualPauseTime_Tests
    {
        private readonly WorkTimeService _workTimeService;

        public WorkTimeService_ActualPauseTime_Tests()
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
        public void NoTimeNoPauseTime()
        {
            var data = new List<CheckInItem>();

            var (_, paused) = _workTimeService.PrepareCheckins(data);

            var res = _workTimeService.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void OneTimeNoPauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 6, 11, 0)}
            };

            var (_, paused) = _workTimeService.PrepareCheckins(data);

            var res = _workTimeService.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void TwoTimesNoPauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)}
            };
            var (_, paused) = _workTimeService.PrepareCheckins(data);

            var res = _workTimeService.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void ThreeTimesPauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
            };
            var (_, paused) = _workTimeService.PrepareCheckins(data);

            var res = _workTimeService.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(17));
        }

        [Fact]
        public void FourTimesPauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
            };
            var (_, paused) = _workTimeService.PrepareCheckins(data);

            var res = _workTimeService.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(17));
        }

        [Fact]
        public void FiveTimesPauseTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
            };
            var (_, paused) = _workTimeService.PrepareCheckins(data);

            var res = _workTimeService.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(31));
        }

        [Fact]
        public void SixTimesNoWorkTime()
        {
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 37, 0)}
            };
            var (_, paused) = _workTimeService.PrepareCheckins(data);

            var res = _workTimeService.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(31));
        }
    }
}
