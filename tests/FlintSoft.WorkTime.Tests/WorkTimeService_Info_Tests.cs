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
    public class WorkTimeService_Info_Tests
    {
        private readonly WorkTimeConfig _cfg;

        public WorkTimeService_Info_Tests()
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
        
        //[Fact]
        public async Task FullWorkDayNoOvertime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var workDay = new DateTime(2021, 9, 22);
            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 0, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 14, 32, 0)}
            };

            var res = sut.GetWorkTimeInfo(workDay, data);

            await Verifier.Verify(res);

            res.Day.Should().Be(new DateTime(2021, 9, 22));
            res.Time2GoHome.Should().Be(new DateTime(2021, 9, 22, 14, 32, 0));
        }
    }
}
