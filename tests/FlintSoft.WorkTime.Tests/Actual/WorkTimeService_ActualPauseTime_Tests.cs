using FlintSoft.Tools.Feiertage;
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
        private readonly WorkTimeConfig _cfg;

        public WorkTimeService_ActualPauseTime_Tests()
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
        public void NoTimeNoPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>();

            var (_, paused) = sut.PrepareCheckins(data);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void OneTimeNoPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 7, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 6, 11, 0)}
            };

            var (_, paused) = sut.PrepareCheckins(data);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void TwoTimesNoPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 19, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)}
            };
            var (_, paused) = sut.PrepareCheckins(data);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void ThreeTimesPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 36, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
            };
            var (_, paused) = sut.PrepareCheckins(data);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(17));
        }

        [Fact]
        public void FourTimesPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
            };
            var (_, paused) = sut.PrepareCheckins(data);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(17));
        }

        [Fact]
        public void FiveTimesPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
            };
            var (_, paused) = sut.PrepareCheckins(data);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(31));
        }

        [Fact]
        public void SixTimesNoWorkTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 16, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 37, 0)}
            };
            var (_, paused) = sut.PrepareCheckins(data);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(31));
        }

        [Fact]
        public void NoTimeNoPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>();

            var (_, paused) = sut.PrepareCheckins(data, true);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void OneTimeNoPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 7, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)}
            };

            var (_, paused) = sut.PrepareCheckins(data, true);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void TwoTimesNoPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 19, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)}
            };
            var (_, paused) = sut.PrepareCheckins(data, true);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromHours(0));
        }

        [Fact]
        public void ThreeTimesPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 36, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
            };
            var (_, paused) = sut.PrepareCheckins(data, true);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(17));
        }

        [Fact]
        public void FourTimesPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
            };
            var (_, paused) = sut.PrepareCheckins(data, true);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(17));
        }

        [Fact]
        public void FiveTimesPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
            };
            var (_, paused) = sut.PrepareCheckins(data, true);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(31));
        }

        [Fact]
        public void SixTimesNoWorkTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 16, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 37, 0)}
            };
            var (_, paused) = sut.PrepareCheckins(data, true);

            var res = sut.CalculatePauseTime(paused);

            res.Should().Be(TimeSpan.FromMinutes(31));
        }
    }
}
