using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace FlintSoft.WorkTime.Tests
{
    [UsesVerify]
    public class WorkTimeService_Info_Tests
    {
        private readonly IWorkTimeService _workTimeService;

        public WorkTimeService_Info_Tests()
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
        public async Task FullWorkDayNoOvertime()
        {
            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 14, 32, 0)}
            };

            var res = _workTimeService.GetWorkTimeInfo(workDay, data);

            await Verifier.Verify(res);

            res.Day.Should().Be(new DateTime(2021, 9, 22));
            res.Time2GoHome.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }
    }
}
