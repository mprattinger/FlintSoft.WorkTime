using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FlintSoft.WorkTime.Tests.ETG
{
    public class WorkTimeService_CalculateTimeToGoHome_Tests
    {
        private readonly WorkTimeConfig _cfg;

        public WorkTimeService_CalculateTimeToGoHome_Tests()
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
        public void NoTTGHWhenNoTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>();

            var info = prepareData(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void NoTTGHWhenNoTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 08, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>();

            var info = prepareDataWithCurrent(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void TTGHWhenStartTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 7, 0, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)}
            };

            var info = prepareData(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [Fact]
        public void TTGHWhenStartTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 7, 0, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)}
            };

            var info = prepareDataWithCurrent(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [Fact]
        public void TTGHWhenTwoTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 0, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)}
            };

            var info = prepareData(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [Fact]
        public void TTGHWhenTwoTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 0, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)}
            };

            var info = prepareDataWithCurrent(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [Fact]
        public void TTGHWhenTenMinPause()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 10, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 10, 0)}
            };

            var info = prepareData(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhenTenMinPauseWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 10, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 10, 0)}
            };

            var info = prepareDataWithCurrent(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhen20MinPause()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 20, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 20, 0)}
            };

            var info = prepareData(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhen20MinPauseWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 20, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 20, 0)}
            };

            var info = prepareDataWithCurrent(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhen30MinPause()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 30, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 30, 0)}
            };

            var info = prepareData(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhen30MinPauseWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 30, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 30, 0)}
            };

            var info = prepareDataWithCurrent(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhen40MinPause()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 40, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 40, 0)}
            };

            var info = prepareData(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [Fact]
        public void TTGHWhen40MinPauseWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 40, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 40, 0)}
            };

            var info = prepareDataWithCurrent(sut, workDay, data);

            var res = sut.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private WorkTimeInfo prepareData(WorkTimeService wService, DateTime workDay, List<CheckInItem> data)
        {
            var info = new WorkTimeInfo
            {
                TargetWorkTime = wService.GetWorkTimeTargetForDay(workDay),
            };
            info.TargetPauseTime = wService.GetTargetPauseForTimeSpan(info.TargetWorkTime, workDay.DayOfWeek == DayOfWeek.Friday);

            var (worked, paused) = wService.PrepareCheckins(data);
            
            info.WorkedTime = wService.CalculateWorkTime(worked);
            
            info.PausedTime = wService.CalculatePauseTime(paused);
            if(info.PausedTime >= TimeSpan.FromMinutes(10))
            {
                //Ab 10 Minuten Pause werden von der Firma 10 Minuten geschenkt
                info.TargetPauseTime = info.TargetPauseTime.Subtract(TimeSpan.FromMinutes(10));
            }
            
            info.IsActive = wService.IsActive(data);
            
            info.StartOfWork = data.FirstOrDefault() == null ? DateTime.MinValue : data.FirstOrDefault().CheckinTime;



            return info;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private WorkTimeInfo prepareDataWithCurrent(WorkTimeService wService, DateTime workDay, List<CheckInItem> data)
        {
            var info = new WorkTimeInfo
            {
                TargetWorkTime = wService.GetWorkTimeTargetForDay(workDay),
            };
            info.TargetPauseTime = wService.GetTargetPauseForTimeSpan(info.TargetWorkTime, workDay.DayOfWeek == DayOfWeek.Friday);

            var (worked, paused) = wService.PrepareCheckins(data, true);

            info.WorkedTime = wService.CalculateWorkTime(worked);

            info.PausedTime = wService.CalculatePauseTime(paused);
            if (info.PausedTime >= TimeSpan.FromMinutes(10))
            {
                //Ab 10 Minuten Pause werden von der Firma 10 Minuten geschenkt
                info.TargetPauseTime = info.TargetPauseTime.Subtract(TimeSpan.FromMinutes(10));
            }

            info.IsActive = wService.IsActive(data);

            info.StartOfWork = data.FirstOrDefault() == null ? DateTime.MinValue : data.FirstOrDefault().CheckinTime;



            return info;
        }
    }
}
