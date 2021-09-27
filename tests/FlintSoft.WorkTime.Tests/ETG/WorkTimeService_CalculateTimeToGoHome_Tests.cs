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

namespace FlintSoft.WorkTime.Tests.ETG
{
    public class WorkTimeService_CalculateTimeToGoHome_Tests
    {
        private readonly IWorkTimeService _workTimeService;

        public WorkTimeService_CalculateTimeToGoHome_Tests()
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
        public void NoTTGHWhenNoTime()
        {
            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>();

            var info = prepareData(workDay, data);

            var res = _workTimeService.CalculateTimeToGoHome(info);

            res.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void TTGHWhenStartTime()
        {
            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)}
            };

            var info = prepareData(workDay, data);

            var res = _workTimeService.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [Fact]
        public void TTGHWhenTwoTime()
        {
            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)}
            };

            var info = prepareData(workDay, data);

            var res = _workTimeService.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [Fact]
        public void TTGHWhenTenMinPause()
        {
            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 10, 0)}
            };

            var info = prepareData(workDay, data);

            var res = _workTimeService.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhen20MinPause()
        {
            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 20, 0)}
            };

            var info = prepareData(workDay, data);

            var res = _workTimeService.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhen30MinPause()
        {
            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 30, 0)}
            };

            var info = prepareData(workDay, data);

            var res = _workTimeService.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }

        [Fact]
        public void TTGHWhen40MinPause()
        {
            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 40, 0)}
            };

            var info = prepareData(workDay, data);

            var res = _workTimeService.CalculateTimeToGoHome(info);

            res.Should().Be(new DateTime(2021, 9, 22, 14, 42, 0));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private WorkTimeInfo prepareData(DateTime workDay, List<CheckInItem> data)
        {
            var info = new WorkTimeInfo
            {
                TargetWorkTime = _workTimeService.GetWorkTimeTargetForDay(workDay),
            };
            info.TargetPauseTime = _workTimeService.GetTargetPauseForTimeSpan(info.TargetWorkTime, workDay.DayOfWeek == DayOfWeek.Friday);

            var (worked, paused) = _workTimeService.PrepareCheckins(data);
            
            info.WorkedTime = _workTimeService.CalculateWorkTime(worked);
            
            info.PausedTime = _workTimeService.CalculatePauseTime(paused);
            if(info.PausedTime >= TimeSpan.FromMinutes(10))
            {
                //Ab 10 Minuten Pause werden von der Firma 10 Minuten geschenkt
                info.TargetPauseTime = info.TargetPauseTime.Subtract(TimeSpan.FromMinutes(10));
            }
            
            info.IsActive = _workTimeService.IsActive(data);
            
            info.StartOfWork = data.FirstOrDefault() == null ? DateTime.MinValue : data.FirstOrDefault().CheckinTime;



            return info;
        }
    }
}
