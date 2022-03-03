using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FlintSoft.WorkTime.Tests.Actual
{
    public class WorkTimeService_IsActive_Tests
    {
        private readonly WorkTimeConfig _cfg;

        //private readonly IWorkTimeService _workTimeService;

        public WorkTimeService_IsActive_Tests()
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

            //_workTimeService = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), cfg);
        }

        [Fact]
        public void NoTimeNotActive()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 8, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>();

            var res = sut.IsActive(data);

            res.Should().BeFalse();
        }

        [Fact]
        public void OneTimeActive()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 7, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22,6,11,0)}
            };

            var res = sut.IsActive(data);

            res.Should().BeTrue();
        }

        [Fact]
        public void TwoTimesNotActive()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 20, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22,6, 11,0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22,11,20,0)}
            };

            var res = sut.IsActive(data);

            res.Should().BeFalse();
        }

        [Fact]
        public void ThreeTimesActive()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 43, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22,6,11,0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11,20,0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11,43,0)}
            };

            var res = sut.IsActive(data);

            res.Should().BeTrue();
        }
    }
}
