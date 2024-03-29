﻿using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FlintSoft.WorkTime.Tests.Actual
{
    public class WorkTimeService_ActualWorkTime_Tests
    {
        private WorkTimeConfig _cfg;

        public WorkTimeService_ActualWorkTime_Tests()
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
        public void NoTimeNoWorkTime()
        {
            var systemTime = new MockSystemTime(DateTime.Now);

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>();

            var (worked, _) = sut.PrepareCheckins(data);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void NoTimeNoWorkTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(DateTime.Now);

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>();

            var (worked, _) = sut.PrepareCheckins(data, true);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void OneTimeNoWorkTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 9, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 6, 11, 0)}
            };

            var (worked, _) = sut.PrepareCheckins(data);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void OneTimeNoWorkTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 23, 9, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 23), CheckinTime = new DateTime(2021, 09, 23, 6, 11, 0)}
            };

            var (worked, _) = sut.PrepareCheckins(data, true);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromHours(3));
        }

        [Fact]
        public void TwoTimesWorkTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)}
            };
            var (worked, _) = sut.PrepareCheckins(data);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(286));
        }

        [Fact]
        public void TwoTimesWorkTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)}
            };
            var (worked, _) = sut.PrepareCheckins(data, true);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(286));
        }

        [Fact]
        public void ThreeTimesWorkTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 36, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
            };
            var (worked, _) = sut.PrepareCheckins(data);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(286));
        }

        [Fact]
        public void ThreeTimesWorkTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 36, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
            };
            var (worked, _) = sut.PrepareCheckins(data, true);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(346));
        }

        [Fact]
        public void FourTimesWorkTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
            };
            var (worked, _) = sut.PrepareCheckins(data);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(477));
        }

        [Fact]
        public void FourTimesWorkTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
            };
            var (worked, _) = sut.PrepareCheckins(data, true);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(477));
        }

        [Fact]
        public void FiveTimesWorkTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 16, 01, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
            };
            var (worked, _) = sut.PrepareCheckins(data);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(477));
        }

        [Fact]
        public void FiveTimesWorkTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 16, 01, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
            };
            var (worked, _) = sut.PrepareCheckins(data, true);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(537));
        }

        [Fact]
        public void SixTimesWorkTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 16, 0, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 37, 0)}
            };
            var (worked, _) = sut.PrepareCheckins(data);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(513));
        }

        [Fact]
        public void SixTimesWorkTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 16, 0, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 37, 0)}
            };
            var (worked, _) = sut.PrepareCheckins(data, true);

            var res = sut.CalculateWorkTime(worked);

            res.Should().Be(TimeSpan.FromMinutes(513));
        }
    }
}
